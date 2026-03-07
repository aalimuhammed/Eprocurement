using AutoMapper;
using EPROCUREMENT.DTO;
using EPROCUREMENT.Infrastructure.Persistence;
using EPROCUREMENT.Models;
using EPROCUREMENT.Services.Interfaces;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
    public class VendorUploadOffer : IUploadOffer
    {
        private readonly ProcurementDBContext procurementDBContext;

        private readonly IMapper _mapper;
        public VendorUploadOffer(ProcurementDBContext procurementDB , IMapper mapper)
        {
            this.procurementDBContext = procurementDB;
            this._mapper = mapper;
        }

        public async Task<int> UploadUserOffer(PackageOfferDTO packages_Offer)
        {
            try
            {
                var pack_mapped = _mapper.Map<users_offers>(packages_Offer);
                var folderName = Path.Combine("Resources", "Offers");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                string dbPath = "";
                string fileName = "";
                if (packages_Offer.File.Length > 0)
                {
                    fileName = ContentDispositionHeaderValue.Parse(packages_Offer.File.ContentDisposition).FileName.Trim('"');

                    var fullPath = Path.Combine(pathToSave, fileName);
                    dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        packages_Offer.File.CopyTo(stream);
                    }



                }


                // packages_Offer.file_name = "A";
                // packages_Offer.file_path = "B";
                //// packages_Offer.user_id = 3;

                procurementDBContext.Attach(pack_mapped);

                procurementDBContext.Entry(pack_mapped).State = Microsoft.EntityFrameworkCore.EntityState.Added;

            //    procurementDBContext.packages_offers.Add(pack_mapped);

                var result = await procurementDBContext.SaveChangesAsync();

                return result;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
