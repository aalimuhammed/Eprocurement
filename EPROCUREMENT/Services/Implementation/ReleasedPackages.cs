using EPROCUREMENT.DTO;
using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.sap;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.ViewModel;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cmp;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
	public class ReleasedPackages : IReleasedPackages
	{
		private readonly ProcurementDBContext _procurementDBContext;
		private readonly IEmailService _emailService;

        public ReleasedPackages(ProcurementDBContext procurementDBContext, IEmailService emailService)
        {
			_procurementDBContext = procurementDBContext;
			_emailService = emailService;
        }

        public async Task<int> CancelReleasedPackage(int pkg_id , CancellationToken cancellationToken)
        {
			var pkg_header = await _procurementDBContext.packages_header
								.Where(x => x.id == pkg_id)
								.FirstOrDefaultAsync(cancellationToken);

			var pkg_details = await _procurementDBContext.packages_details
									.Where(x => x.pkg_id == pkg_id)
									.ToListAsync(cancellationToken);

			pkg_header.cancelled = true;
			_procurementDBContext.Entry(pkg_header).State = EntityState.Modified;

			int result = await _procurementDBContext.SaveChangesAsync(cancellationToken);
            var userIds = pkg_details.Select(x => x.user_id).Distinct();

            var users = await _procurementDBContext.user_header
                .Where(u => userIds.Contains(u.id))
                .Select(u => new { u.id, u.fname, u.email })
                .ToListAsync(cancellationToken);

            if (result > 0)
			{
				foreach (var item in pkg_details)
				{

					SapCancelRevokeDTO sapCancelRevokeDTO = new SapCancelRevokeDTO();
					sapCancelRevokeDTO.pr_num = int.Parse(item.pr_num);
					sapCancelRevokeDTO.line_item = int.Parse(item.line_item);

					await SapApi.CancelRevokeSAP(sapCancelRevokeDTO);
				}
			}

            var emailTasks = users.Select(user =>
            {
                string subject = "SIAC Cancelled Package Assigned";

                string body = $@"
							<html>
							<body style='font-family: Arial, sans-serif; font-size: 14px; line-height: 1.6;'>

							<!-- English Section -->
							<p>Dear {user.fname},</p>

							<br/>

							<p>
							Please be informed that the below Request for Quotation (RFQ) has now been cancelled.
							</p>

							<p>
							{pkg_header.pkg_name}
							</p>

							<br/>

							<p>
							No further action is required from your side regarding this RFQ.
							</p>

							<br/><hr/><br/>

							<!-- Arabic Section -->
							<div style='direction: rtl; text-align: right; font-family: Arial, sans-serif;'>

							<p>السيد/ {user.fname}</p>

							<br/>

							<p>
							يرجى العلم بأنه تم إلغاء طلب عرض السعر (RFQ) الذي تم إنشاؤه سابقًا من قبل شركة سياك للإنشاءات.
							</p>


							<p>
							{pkg_header.pkg_name}
							</p>

							<br/>

							<p>
							لا يلزم اتخاذ أي إجراء من جانبكم بخصوص هذا الطلب.
							</p>

							</div>

							<br/>

							<p>
							Best Regards,<br/>
							Procurement Team
							</p>

							</body>
							</html>";

                return _emailService.SendEmailAsync(user.email, subject, body);
            });

            await Task.WhenAll(emailTasks);

            return result;
        }

        public async Task<List<packages_header>> GetAllReleased(int vendor_id)
        {
            var industries = await GetVendorIndustries(vendor_id);

			var accepted_offers = await (from a in _procurementDBContext.accepted_offers 
										 select a.pkg_header_id).ToListAsync();

			var vendor_area = await (from a in _procurementDBContext.user_header
									 where a.id  == vendor_id
									 select a.areas_id).FirstOrDefaultAsync();

			var pkgs = await (from ph in _procurementDBContext.packages_header
							  join pd in _procurementDBContext.packages_details on ph.id equals pd.pkg_id
							  join sp in _procurementDBContext.sap_projects on ph.project_id equals sp.Id
							 // where industries.Contains(ph.industry_id) &&
								where !ph.cancelled && !accepted_offers.Contains(ph.id) && sp.area_id == vendor_area
							  select ph.id).Distinct().ToListAsync();



			var pkg_details = await (from pd in _procurementDBContext.packages_details
									 where pkgs.Contains(pd.pkg_id) 
									 select pd.id).ToListAsync();


			#region CheckifTheUserHasBiddedForPkg


			var vendor_biddings = await (from vb in _procurementDBContext.vendor_biddings
										 join pd in _procurementDBContext.packages_details on vb.pkg_details_id equals pd.id 
										 join ph in _procurementDBContext.packages_header on pd.pkg_id equals ph.id
								         where pkg_details.Contains(vb.pkg_details_id) &&
										 vb.user_id == vendor_id
                                   select ph.id).Distinct().ToListAsync();

            if (vendor_biddings.Count > 0)
            {
				// will get both M and Automatic
                var header_query = await (from ph in _procurementDBContext.packages_header
                                    join pd in _procurementDBContext.packages_details on ph.id equals pd.pkg_id
							       join sp in _procurementDBContext.sap_projects on ph.project_id equals sp.Id
                                    where !ph.cancelled && !vendor_biddings.Contains(ph.id) && 
									!accepted_offers.Contains(ph.id) && sp.area_id == vendor_area
                                          select ph).Distinct().ToListAsync();

                bool hasAssignTypeM = header_query.Any(ph => ph.assign_type == 'M');

                if (hasAssignTypeM)
				{
					var manualpkgs =  (from header in header_query
											join pd in _procurementDBContext.packages_details on header.id equals pd.pkg_id
											where pd.user_id == vendor_id
											select header).Distinct().ToList();

					if (manualpkgs.Count == 0)
					{
                        var automaticPkgs = (from header in header_query
                                          where header.assign_type == 'N'
                                          select header).Distinct().ToList();
						return automaticPkgs;
                    }

					return manualpkgs;
				}
				else
				{
					return header_query;
				}


			}
            #endregion

            else
            {
                var query = await (from ph in _procurementDBContext.packages_header
                             join pd in _procurementDBContext.packages_details on ph.id equals pd.pkg_id
                             join sp in _procurementDBContext.sap_projects on ph.project_id equals sp.Id
                                   where !ph.cancelled  // industries.Contains(ph.industry_id)
														  && !accepted_offers.Contains(ph.id) && sp.area_id == vendor_area
                                   select ph).Distinct().ToListAsync();

				bool allAssignTypesMatchManual = query.All(ph => ph.assign_type == 'M');

				if (allAssignTypesMatchManual)
				{
					var manual_assign = (from ph in query
											   join pd in _procurementDBContext.packages_details on ph.id equals pd.pkg_id
											   where pd.user_id == vendor_id
											   select ph).Distinct().ToList();

					return manual_assign;
				}

				bool allAssignTypesMatchAutomatic = query.All(ph => ph.assign_type == 'N');

				if (allAssignTypesMatchAutomatic)
				{
					var Automaticquery = await (from ph in _procurementDBContext.packages_header
												join pd in _procurementDBContext.packages_details on ph.id equals pd.pkg_id
												join sp in _procurementDBContext.sap_projects on ph.project_id equals sp.Id
												where !ph.cancelled && industries.Contains(ph.industry_id)
																	   && !accepted_offers.Contains(ph.id) && sp.area_id == vendor_area
												select ph).Distinct().ToListAsync();

					return Automaticquery;
				}

				bool hasAssignTypeM = query.Any(ph => ph.assign_type == 'M');
				bool hasAssignTypeN = query.Any(ph => ph.assign_type == 'N');

				bool hasBothMAndN = hasAssignTypeM && hasAssignTypeN;

				if (hasBothMAndN)
                {
					var automatic = await (from ph in _procurementDBContext.packages_header
									   join pd in _procurementDBContext.packages_details on ph.id equals pd.pkg_id
                                       join sp in _procurementDBContext.sap_projects on ph.project_id equals sp.Id
                                           where !ph.cancelled && industries.Contains(ph.industry_id) && !accepted_offers.Contains(ph.id) && sp.area_id == vendor_area
                                           select ph).Distinct().ToListAsync();

					var manual = await (from ph in _procurementDBContext.packages_header
									   join pd in _procurementDBContext.packages_details on ph.id equals pd.pkg_id
                                        join sp in _procurementDBContext.sap_projects on ph.project_id equals sp.Id
                                        where !ph.cancelled && /*industries.Contains(ph.industry_id)*/  pd.user_id == vendor_id && !accepted_offers.Contains(ph.id) && sp.area_id == vendor_area
                                        select ph).Distinct().ToListAsync();

					if (manual.Count == 0)
					{
						var automaticPkg = (from a in automatic
											where a.assign_type == 'N'
											select a).ToList();
						return automaticPkg;
					}

					automatic = (from at in automatic
								 where at.assign_type == 'N'
								 select at).ToList();

					var mixed = automatic.Concat(manual).Distinct().ToList();


					return mixed;
				}
				return null;

            }

        }

        public async Task<List<packages_header>> GetAppliedPackages(
			int project_id, 
			int industry_id,
			CancellationToken cancellationToken)
        {
            var acceptedOffersQuery = _procurementDBContext.accepted_offers
						 .Select(x => x.pkg_header_id);

            var query = _procurementDBContext.packages_header
                .Where(x => x.project_id == project_id &&
                            x.bid == true &&
                            !acceptedOffersQuery.Contains(x.id));

            if (industry_id > 0)
            {
                query = query.Where(x => x.industry_id == industry_id);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IQueryable<released_packages_vm>> GetPackages_Details(int pkg_id)
		{
			return await Task.Run(() => _procurementDBContext.Released_Packages_Vms.FromSqlRaw("CALL released_pkgs({0});" , pkg_id));
		}

		public async Task<List<packages_header>> GetPackages_Headers(
			int project_id, int industry_id , CancellationToken cancellationToken)
		{
            var query = _procurementDBContext.packages_header
					   .Where(x => x.project_id == project_id &&
								   x.cancelled != true);

            if (industry_id > 0)
            {
                query = query.Where(x => x.industry_id == industry_id);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<List<packages_header>> GetPriceComparison(
			int project_id, int industry_id , CancellationToken cancellationToken)
        {
            var query = _procurementDBContext.packages_header
							.Where(x => x.project_id == project_id &&
										x.bid == true);

            if (industry_id > 0)
            {
                query = query.Where(x => x.industry_id == industry_id);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IQueryable<released_headers_vm>> GetReleased_Headers(int project_id, int industry_id)
        {
            return await Task.Run(() => _procurementDBContext.Released_Headers_Vms.FromSqlRaw("CALL released_headers({0}, {1});", project_id , industry_id));
        }

        public async Task<List<packages_details>> GetReleased_Packages(
			int pkg_id ,
			CancellationToken cancellationToken = default)
		{
            var result = await _procurementDBContext.packages_details
                                .Where(x => x.pkg_id == pkg_id)
                                .ToListAsync(cancellationToken);

			var groupedData = result.GroupBy(x => new
			                                {
				                                x.pkg_id,
				                                x.pr_num,
				                                x.line_item,
				                                x.pr_date,
				                                x.mtr_code,
				                                x.mtr_desc,
				                                x.mtr_long_desc,
				                                x.mtr_batch,
				                                x.mtr_qty,
				                                x.m_grp,
				                                x.mtr_uom
			                                }).Select(y => new
			                                {
				                                Key = y.Key,
				                                packages_details = y.ToList()
			                                }).ToList();


			var packagesDetailsList = groupedData.SelectMany(y => y.packages_details)
                                                  .DistinctBy(x => new
                                                  {
                                                      x.pkg_id,
                                                      x.pr_num,
                                                      x.line_item,
                                                      x.mtr_code
                                                  })
                                                  .ToList();

			return packagesDetailsList;
		}

        public async Task<List<released_packages>> GetSelectedPackages(int project_id, int industry_id)
        {
            var result = await _procurementDBContext.released_packages
                .Where(x => x.project_id == project_id && 
				          x.industry_id == industry_id)
                .ToListAsync();

            return result;
        }
        public async Task<List<int>> GetVendorIndustries(int vendor_id)
        {
			var industries = await _procurementDBContext.user_detail
								.Where(x => x.user_id == vendor_id)
								.Join(_procurementDBContext.user_header,
									  detail => detail.user_id,
									  header => header.id,
									  (detail, header) => new { detail.industries_details, header })
								.Where(joined => joined.header.verifyied == true)
								.Select(joined => joined.industries_details)
								.ToListAsync();
			return industries;
        }

        public async Task<IQueryable<vendors_vms>> GetVendors(int pkg_id)
        {
            return await Task.Run(() => _procurementDBContext.Vendors_Vms.FromSqlRaw("CALL assigned_vendors({0});", pkg_id));
        }
        public async Task<List<UserAssignedDTO>> GetUserAssigned(int pkg_id, CancellationToken cancellationToken = default)
        {
            var usersAssigned = await _procurementDBContext.packages_details
								   .Where(x => x.pkg_id == pkg_id)
								   .Join(_procurementDBContext.user_header,
										 detail => detail.user_id,
										 header => header.id,
										 (detail, header) => new { detail, header })

								   .Select(x => new UserAssignedDTO
								   {
									   UserId = x.header.id,
									   TaxId = x.header.tax_id,
									   CompanyName = x.header.company
								   })
								   .Distinct()
								   .ToListAsync(cancellationToken);

            return usersAssigned;
        }
        public async Task<int> DeleteAssignedUser(
			int pkg_id, 
			int user_id, 
			CancellationToken cancellationToken = default)
        {
            var userAssigned = await _procurementDBContext.packages_details
                                 .Where(x => x.pkg_id == pkg_id && x.user_id == user_id)
                                 .FirstOrDefaultAsync(cancellationToken);
			
            if (userAssigned != null)
            {
                _procurementDBContext.packages_details.Remove(userAssigned);
                await _procurementDBContext.SaveChangesAsync(cancellationToken);

                var userEmails = await _procurementDBContext.user_header
                    .Where(u => u.id == user_id)
                    .Select(u => new
                    {
                        u.email,
                        u.fname,
                    })
                    .ToListAsync(cancellationToken);


                var pkgName = await _procurementDBContext.packages_header
                                .Where(ph => ph.id == pkg_id)
                                .Select(ph => ph.pkg_name)
                                .FirstOrDefaultAsync(cancellationToken);

                var emailTasks = userEmails.Select(user =>
                {
                    string subject = "SIAC Cancelled Package Assigned";

                    string body = $@"
							<html>
							<body style='font-family: Arial, sans-serif; font-size: 14px; line-height: 1.6;'>

							<!-- English Section -->
							<p>Dear {user.fname},</p>

							<br/>

							<p>
							Please be informed that the below Request for Quotation (RFQ) has now been cancelled.
							</p>

							<p>
							{pkgName}
							</p>

							<br/>

							<p>
							No further action is required from your side regarding this RFQ.
							</p>

							<br/><hr/><br/>

							<!-- Arabic Section -->
							<div style='direction: rtl; text-align: right; font-family: Arial, sans-serif;'>

							<p>السيد/ {user.fname}</p>

							<br/>

							<p>
							يرجى العلم بأنه تم إلغاء طلب عرض السعر (RFQ) الذي تم إنشاؤه سابقًا من قبل شركة سياك للإنشاءات.
							</p>


							<p>
							{pkgName}
							</p>

							<br/>

							<p>
							لا يلزم اتخاذ أي إجراء من جانبكم بخصوص هذا الطلب.
							</p>

							</div>

							<br/>

							<p>
							Best Regards,<br/>
							Procurement Team
							</p>

							</body>
							</html>";

                    return _emailService.SendEmailAsync(user.email, subject, body);
                });

                await Task.WhenAll(emailTasks);
                return 1;
            }

            return 0;
        }
		public async Task<int> AssignUserToPackage(
			int pkg_id,
			int user_id,
			CancellationToken cancellationToken = default)
		{
			var packagesDetails = await _procurementDBContext.packages_details
								 .Where(x => x.pkg_id == pkg_id)
								 .FirstOrDefaultAsync(cancellationToken);

			if (packagesDetails != null)
			{
				var newAssignment = new packages_details
				{
					pkg_id = pkg_id,
					user_id = user_id,
					pr_num = packagesDetails.pr_num,
					line_item = packagesDetails.line_item,
					pr_date = packagesDetails.pr_date,
					mtr_code = packagesDetails.mtr_code,
					mtr_desc = packagesDetails.mtr_desc,
					mtr_long_desc = packagesDetails.mtr_long_desc,
					mtr_batch = packagesDetails.mtr_batch,
					mtr_qty = packagesDetails.mtr_qty,
					m_grp = packagesDetails.m_grp,
					mtr_uom = packagesDetails.mtr_uom,
					serial = packagesDetails.serial
                };

				await _procurementDBContext.packages_details.AddAsync(newAssignment, cancellationToken);
				await _procurementDBContext.SaveChangesAsync(cancellationToken);

				var pkgName = await _procurementDBContext.packages_header
								.Where(ph => ph.id == pkg_id)
								.Select(ph => ph.pkg_name)
								.FirstOrDefaultAsync(cancellationToken);

                var userEmails = await _procurementDBContext.user_header
                                .Where(u => u.id == user_id)
                                .Select(u => new
                                {
                                    u.email,
                                    u.fname,
                                })
                                .ToListAsync(cancellationToken);

                var emailTasks = userEmails.Select(user =>
                {
                    string subject = "SIAC New Package Assigned";

                    string body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; font-size: 14px; line-height: 1.6;'>

                        <!-- English Section -->
                        <p>Dear {user.fname},</p>

                        <br/>

                        <p>
                        Please be informed that a new Request for Quotation (RFQ) has been initiated by SIAC Construction Company and has been assigned to you.
                        </p>

                        <p><b>Package details are as follows:</b></p>

                        <p>
                        {pkgName}
                        </p>

                        <br/>

                        <p>
                        Kindly review the RFQ and submit your quotation accordingly.<br/>
                        For further details, please refer to the following link:<br/>
                        <a href='https://eproc.siac-construction.com:9443/'>
                        https://eproc.siac-construction.com:9443/
                        </a>
                        </p>

                        <br/><hr/><br/>

                        <!-- Arabic Section -->
                        <div style='direction: rtl; text-align: right; font-family: Arial, sans-serif;'>

                        <p>السيد/ {user.fname}</p>

                        <br/>

                        <p>
                        يرجى العلم بأنه تم إنشاء طلب عرض سعر (RFQ) جديد من قبل شركة سياك للإنشاءات، وقد تم إسناده إلى سيادتكم.
                        </p>

                        <p><b>تفاصيل حزمة الأعمال كما يلي:</b></p>

                        <p>
                        {pkgName}
                        </p>

                        <br/>

                        <p>
                        نرجو من سيادتكم مراجعة الطلب وتقديم عرض السعر الخاص بكم في أقرب وقت ممكن.<br/>
                        لمزيد من التفاصيل، يرجى زيارة الرابط التالي:<br/>
                        <a href='https://eproc.siac-construction.com:9443/'>
                        https://eproc.siac-construction.com:9443/
                        </a>
                        </p>

                        </div>

                        <br/>

                        <p>
                        Best Regards,<br/>
                        Procurement Team
                        </p>

                        </body>
                        </html>";

                    return _emailService.SendEmailAsync(user.email, subject, body);
                });

                await Task.WhenAll(emailTasks);
                return 1;
            }
			return 0;
		}
    }
}