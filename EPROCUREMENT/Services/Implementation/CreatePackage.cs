using EPROCUREMENT.DTO;
using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.sap;
using EPROCUREMENT.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
    public class CreatePackage : ICreatePackage
	{
		private readonly ProcurementDBContext _procurementDBContext;
        private readonly IEmailService _emailService;
        public CreatePackage(
            ProcurementDBContext procurementDBContext , 
            IEmailService emailService)
		{
			_procurementDBContext = procurementDBContext;
            _emailService = emailService;
		}

		public async Task<int> CreateSapPackage(CreatePackageDTO createPackageDTO)
		{
			int returnedvalue = 0;

			int maxId = await _procurementDBContext.packages_header.AnyAsync()
						 ? await _procurementDBContext.packages_header.MaxAsync(u => u.id)
						 : 0;


			int pkg_id = await _procurementDBContext.packages_header
								.Where(x => x.project_id == createPackageDTO.project_id
										 && x.industry_id == createPackageDTO.industry_id)
								.AnyAsync()
								? await _procurementDBContext.packages_header
									.Where(x => x.project_id == createPackageDTO.project_id
											 && x.industry_id == createPackageDTO.industry_id)
									.MaxAsync(u => u.pkg_num)
								: 0;

			string appendedValue = (pkg_id + 1).ToString("00");

			// Concatenate the package name with the formatted appended value
			string pkgName = createPackageDTO.package_name + "/" + appendedValue;
			int index = createPackageDTO.package_name.IndexOf('/');
			string industryResult = index >= 0 ? createPackageDTO.package_name.Substring(index + 1) : createPackageDTO.package_name;


			// Create a new packages_header instance
			var package = new packages_header
			{
				pkg_name = pkgName,
				assign_type = createPackageDTO.assign_type,
				project_id = createPackageDTO.project_id,
				industry_id = createPackageDTO.industry_id,
				assigned_by = createPackageDTO.assigned_by,
				file_path = createPackageDTO.filePath,
                currency = createPackageDTO.currency,
				pkg_num = pkg_id + 1
			};

			await _procurementDBContext.packages_header.AddAsync(package);
			var header_insert = await _procurementDBContext.SaveChangesAsync();

			if (header_insert > 0)
			{
                var packageDetailsList = new List<packages_details>();
                var sapStatusDTOList = new List<SapStatusDTO>();

                foreach (var item in createPackageDTO.selected_rows.Distinct())
				{
					var package_details = new packages_details();
					package_details.pr_num = item.pr;
					package_details.pkg_id = maxId + 1;
					package_details.pr_date = DateTime.ParseExact(item.pr_req_date, "d-M-yyyy", null);
					package_details.mtr_uom = item.m_uom;
					package_details.mtr_code = item.mat_code;
					package_details.m_grp = item.m_grp;
					package_details.mtr_batch = item.m_batch;
					package_details.mtr_desc = item.mat_desc;
					package_details.line_item = item.pr_item;
					package_details.mtr_qty= item.m_req_qty;
					package_details.mtr_long_desc = item.m_long_desc;
					package_details.user_id = 0;

                    packageDetailsList.Add(package_details);
                    //await _procurementDBContext.packages_details.AddAsync(package_details);

                   // returnedvalue =  await _procurementDBContext.SaveChangesAsync();

                    var SapDTO = new SapStatusDTO();
					SapDTO.EPACKAGE = pkgName;
					SapDTO.PR = item.pr;
					SapDTO.ITEM = int.Parse(item.pr_item);
                    SapDTO.industry = industryResult; 
					SapDTO.STATUS = "Automatic-Released";
                    sapStatusDTOList.Add(SapDTO);

                    await SapApi.SaveToSAPAsync(SapDTO);

                }
                await _procurementDBContext.packages_details.AddRangeAsync(packageDetailsList);
                returnedvalue = await _procurementDBContext.SaveChangesAsync();

                var saveTasks = sapStatusDTOList.Select(dto => SapApi.SaveToSAPAsync(dto));
                await Task.WhenAll(saveTasks);

            }

			return returnedvalue;
		}
		public async Task<int> CreateSapPackageManual(CreatePackageManualDTO createPackageManualDTO)
		{
			int returnedvalue = 0;

			int maxId = await _procurementDBContext.packages_header.AnyAsync()
					 ? await _procurementDBContext.packages_header.MaxAsync(u => u.id)
					 : 0;


			int pkg_id = await _procurementDBContext.packages_header
								.Where(x => x.project_id == createPackageManualDTO.project_id
										 && x.industry_id == createPackageManualDTO.industry_id)
								.AnyAsync()
								? await _procurementDBContext.packages_header
									.Where(x => x.project_id == createPackageManualDTO.project_id
											 && x.industry_id == createPackageManualDTO.industry_id)
									.MaxAsync(u => u.pkg_num)
								: 0;


			string appendedValue = (pkg_id + 1).ToString("00");

			// Concatenate the package name with the formatted appended value
			string pkgName = createPackageManualDTO.package_name + "/" + appendedValue;

			int index = createPackageManualDTO.package_name.IndexOf('/');
			string industryResult = index >= 0 ? createPackageManualDTO.package_name.Substring(index + 1) : createPackageManualDTO.package_name;

			// Create a new packages_header instance
			var package = new packages_header
			{
				pkg_name = pkgName,
				assign_type = createPackageManualDTO.assign_type,
				project_id = createPackageManualDTO.project_id,
				industry_id = createPackageManualDTO.industry_id,
				assigned_by = createPackageManualDTO.assigned_by,
				file_path = createPackageManualDTO.filePath,
                currency = createPackageManualDTO.currency,
				pkg_num = pkg_id + 1
			};

			await _procurementDBContext.packages_header.AddAsync(package);
			var header_insert = await _procurementDBContext.SaveChangesAsync();

			if (header_insert > 0)
			{
				var packageDetails = createPackageManualDTO.selected_rows
					.Distinct()
					.SelectMany(item => createPackageManualDTO.user_id
						.Select(userId => new packages_details
						{
							pr_num = item.pr,
							pkg_id = maxId + 1,
							pr_date = DateTime.ParseExact(item.pr_req_date, "d-M-yyyy", null),
                            mtr_uom = item.m_uom,
							mtr_code = item.mat_code,
							m_grp = item.m_grp,
							mtr_batch = item.m_batch,
							mtr_desc = item.mat_desc,
							line_item = item.pr_item,
							mtr_qty = item.m_req_qty,
							mtr_long_desc = item.m_long_desc,
							user_id = userId
						}));

                await _procurementDBContext.packages_details.AddRangeAsync(packageDetails);
                returnedvalue = await _procurementDBContext.SaveChangesAsync();

                var sapStatusDTOList = new List<SapStatusDTO>();
                foreach (var row in packageDetails)
                {
                    var SapDTO = new SapStatusDTO
                    {
                        EPACKAGE = pkgName,
                        PR = row.pr_num,
                        industry = industryResult,
                        ITEM = int.Parse(row.line_item),
                        STATUS = "Manual-Released"
                    };

                    sapStatusDTOList.Add(SapDTO);
                }

                // save all in parallel
                var saveTasks = sapStatusDTOList.Select(dto => SapApi.SaveToSAPAsync(dto));
                await Task.WhenAll(saveTasks);

                var userEmails = await _procurementDBContext.user_header
                                 .Where(u => createPackageManualDTO.user_id.Contains(u.id))
                                 .Select(u => new
                                 {
                                     u.id,
                                     u.email,
                                     u.fname,
                                 })
                                 .ToListAsync();

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
            }

			return returnedvalue;
		}
        public async Task<int> CreateSapSrvPackage(CreateSrvPackageDTO createSrvPackageDTO)
        {
            int returnedvalue = 0;

            int maxId = await _procurementDBContext.packages_header.Where(p => p.is_service == true).AnyAsync()
                         ? await _procurementDBContext.packages_header.Where(p => p.is_service == true).MaxAsync(u => u.id)
                         : 0;


            int pkg_id = await _procurementDBContext.packages_header
                                .Where(x => x.project_id == createSrvPackageDTO.project_id
								       && x.is_service == true
                                         && x.industry_id == createSrvPackageDTO.industry_id)
                                .AnyAsync()
                                ? await _procurementDBContext.packages_header
                                    .Where(x => x.project_id == createSrvPackageDTO.project_id
                                       && x.is_service == true
                                             && x.industry_id == createSrvPackageDTO.industry_id)
                                    .MaxAsync(u => u.pkg_num)
                                : 0;

            string appendedValue = (pkg_id + 1).ToString("00");

            // Concatenate the package name with the formatted appended value
            string pkgName = createSrvPackageDTO.package_name + "/" + appendedValue;

            // Create a new packages_header instance
            var package = new packages_header
            {
                pkg_name = pkgName,
                assign_type = createSrvPackageDTO.assign_type,
                project_id = createSrvPackageDTO.project_id,
                industry_id = createSrvPackageDTO.industry_id,
                assigned_by = createSrvPackageDTO.assigned_by,
                file_path = createSrvPackageDTO.filePath,
                currency = createSrvPackageDTO.currency,
                is_service = true,
                pkg_num = pkg_id + 1
            };

            await _procurementDBContext.packages_header.AddAsync(package);
            var header_insert = await _procurementDBContext.SaveChangesAsync();

            if (header_insert > 0)
            {
                var packageDetailsList = new List<packages_details>();
                var sapStatusDTOList = new List<SapStatusDTO>();

                foreach (var item in createSrvPackageDTO.selected_rows.Distinct())
                {
                    var package_details = new packages_details();
                    package_details.pr_num = item.PR;
                    package_details.pkg_id = maxId + 1;
                    DateTime date = DateTime.ParseExact(item.PR_REQ_DATE, "d-M-yyyy", null);
                    package_details.pr_date = date;
                    package_details.mtr_uom = item.S_UNIT;
                    package_details.mtr_code = item.SER_CODE;
                    package_details.m_grp = item.S_GRP;
                 //   package_details.mtr_batch = item.;
                    package_details.mtr_desc = item.SER_DESC;
                    package_details.line_item = item.PR_ITEM;
                    package_details.mtr_qty = item.S_REQ_QTY;
                    package_details.mtr_long_desc = item.S_LON_DES;
                    package_details.user_id = 0;

                    packageDetailsList.Add(package_details);
                    //await _procurementDBContext.packages_details.AddAsync(package_details);

                    // returnedvalue =  await _procurementDBContext.SaveChangesAsync();

                    var SapDTO = new SapStatusDTO();
                    SapDTO.EPACKAGE = pkgName;
                    SapDTO.PR = item.PR;
                    SapDTO.ITEM = int.Parse(item.PR_ITEM);
                    SapDTO.STATUS = "Automatic-Released";
                    sapStatusDTOList.Add(SapDTO);

                    await SapApi.SaveToSAPAsync(SapDTO);

                }
                await _procurementDBContext.packages_details.AddRangeAsync(packageDetailsList);
                returnedvalue = await _procurementDBContext.SaveChangesAsync();

                var saveTasks = sapStatusDTOList.Select(dto => SapApi.SaveToSAPAsync(dto));
                await Task.WhenAll(saveTasks);

            }

            return returnedvalue;
        }

        public async Task<int> CreateSapSrvPackageManual(CreateSrvPackageDTOManual createSrvPackageDTOManual)
        {
            int returnedvalue = 0;

            int maxId = await _procurementDBContext.packages_header
                               .Where(p => p.is_service == true).AnyAsync()
                         ? await _procurementDBContext.packages_header
                              .Where(p => p.is_service == true).MaxAsync(u => u.id)
                         : 0;


            int pkg_id = await _procurementDBContext.packages_header
                                .Where(x => x.project_id == createSrvPackageDTOManual.project_id
                                       && x.is_service == true
                                         && x.industry_id == createSrvPackageDTOManual.industry_id)
                                .AnyAsync()
                                ? await _procurementDBContext.packages_header
                                    .Where(x => x.project_id == createSrvPackageDTOManual.project_id
                                       && x.is_service == true
                                             && x.industry_id == createSrvPackageDTOManual.industry_id)
                                    .MaxAsync(u => u.pkg_num)
                                : 0;

            string appendedValue = (pkg_id + 1).ToString("00");

            // Concatenate the package name with the formatted appended value
            string pkgName = createSrvPackageDTOManual.package_name + "/" + appendedValue;

            // Create a new packages_header instance
            var package = new packages_header
            {
                pkg_name = pkgName,
                assign_type = createSrvPackageDTOManual.assign_type,
                project_id = createSrvPackageDTOManual.project_id,
                industry_id = createSrvPackageDTOManual.industry_id,
                is_service = true,
                assigned_by = createSrvPackageDTOManual.assigned_by,
                file_path = createSrvPackageDTOManual.filePath,
                currency = createSrvPackageDTOManual.currency,
                pkg_num = pkg_id + 1
            };

            await _procurementDBContext.packages_header.AddAsync(package);
            var header_insert = await _procurementDBContext.SaveChangesAsync();

            if (header_insert > 0)
            {
                var packageDetails = createSrvPackageDTOManual.selected_rows
                    .Distinct()
                    .SelectMany(item => createSrvPackageDTOManual.user_id
                        .Select(userId => new packages_details
                        {
                            pr_num = item.PR,
                            pkg_id = maxId + 1,
                            pr_date = DateTime.ParseExact(item.PR_REQ_DATE, "d-M-yyyy", null),
                            mtr_uom = item.S_UNIT,
                            mtr_code = item.SER_CODE,
                            m_grp = item.S_GRP,
                           // mtr_batch = item.m_batch,
                            mtr_desc = item.SER_DESC,
                            line_item = item.PR_ITEM,
                            mtr_qty = item.S_REQ_QTY,
                            mtr_long_desc = item.S_LON_DES,
                            user_id = userId
                        }));



                await _procurementDBContext.packages_details.AddRangeAsync(packageDetails);
                returnedvalue = await _procurementDBContext.SaveChangesAsync();

                var sapStatusDTOList = new List<SapStatusDTO>();
                foreach (var row in packageDetails)
                {

                    var SapDTO = new SapStatusDTO();
                    SapDTO.EPACKAGE = pkgName;
                    SapDTO.PR = row.pr_num;
                    SapDTO.ITEM = int.Parse(row.line_item);
                    SapDTO.STATUS = "Manual-Released";
                    sapStatusDTOList.Add(SapDTO);
                    await SapApi.SaveToSAPAsync(SapDTO);
                }

                var saveTasks = sapStatusDTOList.Select(dto => SapApi.SaveToSAPAsync(dto));
                await Task.WhenAll(saveTasks);
            }

            return returnedvalue;
        }
    }
}
