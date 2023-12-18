
using EventsClient.Models;
using EventsClient.Models.DTOs;
using EventsClient.Models.ViewModels;
using EventsClients.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace EventsClient.Controllers
{
    public class EventListingController : Controller
    {
        const string API_URL = "https://localhost:7161/api";
        Uri baseAddress2 = new Uri("https://localhost:7161/auth");

        const string API_ENDPOINT = "EventListing";
        const string ADMISSION_ENDPOINT = "Admission";


        Uri baseAddress = new Uri(API_URL);
        private readonly HttpClient _client;
        private readonly HttpClient _client2;
        public EventListingController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
            
            _client2 = new HttpClient();
            _client2.BaseAddress = baseAddress2;
        }

        public IActionResult PassSessionUsername()
        {
            var username = HttpContext.Session.GetString("SessionUsername");
            ViewBag.LogInUser = username;

            return View();
        }

        //Method for token / session
        [HttpGet]
        private string RetrieveTokenFromSession()
        {
            string token = HttpContext.Session.GetString("SessionAuth")!;
            return token;
        }

        static List<T> GetData<T>(HttpClient client, string endpoint, Func<T, bool> filter) where T : class
        {
            var response = client.GetAsync($"{client.BaseAddress}/{endpoint}").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var listing = JsonConvert.DeserializeObject<List<T>>(content);
            return listing.Where(filter).ToList();
        }

        [HttpGet]
        public IActionResult Index()
        {
            string token = RetrieveTokenFromSession();

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PassSessionUsername();

            var response = _client.GetAsync($"{_client.BaseAddress}/{API_ENDPOINT}").Result;
            string data = response.Content.ReadAsStringAsync().Result;
            var eventList = JsonConvert.DeserializeObject<List<EvHeader>>(data)!;

            return View(eventList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            string token = RetrieveTokenFromSession();

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PassSessionUsername();


            EvHeaderCreateDTO item = new EvHeaderCreateDTO();

            item.Admission = new List<Admission>
            {
                new Admission() { Id = 1 }
            };

            item.Music = new List<Music>
            {
                new Music() {Id = 1}
            };

            item.Outlet = new List<Outlet>
            {
                new Outlet() {Id = 1}
            };

            item.Sponsor = new List<Sponsor>
            {
                new Sponsor() {Id = 1}
            };

            var cResponse = _client.GetAsync($"{_client.BaseAddress}/Category").Result;
            var cContent = cResponse.Content.ReadAsStringAsync().Result;
            var categoryData = JsonConvert.DeserializeObject<List<Category>>(cContent);

            item.CategoryList = categoryData.Select(cat => new SelectListItem
            {
                Text = cat.CategoryName,
                Value = cat.Id.ToString()
            }).ToList();

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EvHeaderCreateDTO values)
        {
            string token = RetrieveTokenFromSession();

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PassSessionUsername();

            // Serialize the object and sends it to thru the API to the DB
            var response = await SendListingToApi(values);

            if (response.IsSuccessStatusCode)
            {
                var createdEventJson = await response.Content.ReadAsStringAsync();
                var createdEvent = JsonConvert.DeserializeObject<EvHeaderCreateDTO>(createdEventJson);
                var eventId = createdEvent.Id;

                var admissionResponse = await SendAdmissionToApi(values, eventId);
                var musicResponse = await SendMusicToApi(values, eventId);
                var outletResponse = await SendOutletToApi(values, eventId);
                var sponsorResponse = await SendSponsorToApi(values, eventId);

                return RedirectToAction("Index");
            }
            else
            {
                return View(values);
            }
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            string token = RetrieveTokenFromSession();

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PassSessionUsername();

            //gets the event
            var eventResponse = _client.GetAsync($"{_client.BaseAddress}/{API_ENDPOINT}").Result;
            var eventContent = eventResponse.Content.ReadAsStringAsync().Result;
            var eventListing = JsonConvert.DeserializeObject<List<EvHeader>>(eventContent);
            var currentEvent = eventListing.FirstOrDefault(x => x.Id == Id);

            //gets the admission values
            var admissionResponse = _client.GetAsync($"{_client.BaseAddress}/{ADMISSION_ENDPOINT}").Result;
            var admissionContent = admissionResponse.Content.ReadAsStringAsync().Result;
            var admissionListing = JsonConvert.DeserializeObject<List<Admission>>(admissionContent);
            var currentAdmissions = admissionListing.Where(x => x.EventListingId == currentEvent.Id).ToList();

            //gets the music values
            var musicResponse = _client.GetAsync($"{_client.BaseAddress}/Music").Result;
            var musicContent = musicResponse.Content.ReadAsStringAsync().Result;
            var musicListing = JsonConvert.DeserializeObject<List<Music>>(musicContent);
            var currentMusics = musicListing.Where(x => x.EventListingId == currentEvent.Id).ToList();

            //gets the outlet values
            var outletResponse = _client.GetAsync($"{_client.BaseAddress}/Outlet").Result;
            var outletContent = outletResponse.Content.ReadAsStringAsync().Result;
            var outletListing = JsonConvert.DeserializeObject<List<Outlet>>(outletContent);
            var currentOutlets = outletListing.Where(x => x.EventListingId == currentEvent.Id).ToList();

            //gets the sponsor values
            var sponsorResponse = _client.GetAsync($"{_client.BaseAddress}/Sponsor").Result;
            var sponsorContent = sponsorResponse.Content.ReadAsStringAsync().Result;
            var sponsorListing = JsonConvert.DeserializeObject<List<Sponsor>>(sponsorContent);
            var currentSponsors = sponsorListing.Where(x => x.EventListingId == currentEvent.Id).ToList();

            // gets the category data
            var cResponse = _client.GetAsync($"{_client.BaseAddress}/Category").Result;
            var cContent = cResponse.Content.ReadAsStringAsync().Result;
            var categoryData = JsonConvert.DeserializeObject<List<Category>>(cContent);

            var viewModel = new EvHeaderCreateDTO
            {
                PromoImgString = currentEvent.PromoImage,
                PromoImgString2 = currentEvent.PromoImage2,
                PromoImgString3 = currentEvent.PromoImage3,
                PermitImgString = currentEvent.Permit,
                EventName = currentEvent.EventName,
                EventDesc = currentEvent.EventDesc,
                EventLocation = currentEvent.EventLocation,
                EventDate = currentEvent.EventDate,
                SelectedCategoryId = (int)currentEvent.CategoryId,
                Admission = (List<Admission>)currentAdmissions,
                Sponsor = (List<Sponsor>)currentSponsors,
                Music = (List<Music>)currentMusics,
                Outlet = (List<Outlet>)currentOutlets,

                CategoryList = categoryData.Select(cat => new SelectListItem
                {
                    Text = cat.CategoryName,
                    Value = cat.Id.ToString()
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EvHeaderCreateDTO values)
        {
            string token = RetrieveTokenFromSession();

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PassSessionUsername();

            // Serialize the object and sends it to thru the API to the DB
            var response = await UpdateListingToApi(values);

            if (response.IsSuccessStatusCode)
            {
                var updatedEventJSON = await response.Content.ReadAsStringAsync();
                var updatedEvent = JsonConvert.DeserializeObject<EvHeaderCreateDTO>(updatedEventJSON);
                var eventId = updatedEvent.Id;

                var admissionResponse = await UpdateAdmissionToApi(values, eventId);
                var musicResponse = await UpdateMusicToApi(values, eventId);
                var outletResponse = await UpdateOutletToApi(values, eventId);
                var sponsorResponse = await UpdateSponsorToApi(values, eventId);

                return RedirectToAction("Index");
            }
            else
            {
                return View(values);
            }
        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            string token = RetrieveTokenFromSession();

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PassSessionUsername();

            var eResponse = _client.GetAsync($"{_client.BaseAddress}/{API_ENDPOINT}").Result;
            var eData = eResponse.Content.ReadAsStringAsync().Result;
            var eLists = JsonConvert.DeserializeObject<List<EvHeader>>(eData)!;
            var singleEvent = eLists.FirstOrDefault(s => s.Id == Id);
            var eventID = singleEvent.Id;

            var aResponse = _client.GetAsync($"{_client.BaseAddress}/admission").Result;
            string aData = aResponse.Content.ReadAsStringAsync().Result;
            var aLists = JsonConvert.DeserializeObject<List<Admission>>(aData)!;
            List<Admission> relAdmission = new List<Admission>();

            foreach (var a in aLists)
            {
                if (eventID == a.EventListingId)
                {
                    relAdmission.Add(a);
                }
            }


            var mResponse = _client.GetAsync($"{_client.BaseAddress}/music").Result;
            string mData = mResponse.Content.ReadAsStringAsync().Result;
            var mLists = JsonConvert.DeserializeObject<List<Music>>(mData)!;
            List<Music> relMusic = new List<Music>();

            foreach (var a in mLists)
            {
                if (eventID == a.EventListingId)
                {
                    relMusic.Add(a);
                }
            }

            var oResponse = _client.GetAsync($"{_client.BaseAddress}/outlet").Result;
            string oData = oResponse.Content.ReadAsStringAsync().Result;
            var oLists = JsonConvert.DeserializeObject<List<Outlet>>(oData)!;
            List<Outlet> relOutlet = new List<Outlet>();

            foreach (var a in oLists)
            {
                if (eventID == a.EventListingId)
                {
                    relOutlet.Add(a);
                }
            }

            var sResponse = _client.GetAsync($"{_client.BaseAddress}/sponsor").Result;
            string sData = sResponse.Content.ReadAsStringAsync().Result;
            var sLists = JsonConvert.DeserializeObject<List<Sponsor>>(sData)!;
            List<Sponsor> relSponsor = new List<Sponsor>();

            foreach (var a in sLists)
            {
                if (eventID == a.EventListingId)
                {
                    relSponsor.Add(a);
                }
            }

            DetailsVM detailsVM = new DetailsVM();
            {
                detailsVM.Events = singleEvent;
                detailsVM.Admissions = relAdmission;
                detailsVM.Musics = relMusic;
                detailsVM.Outlets = relOutlet;
                detailsVM.Sponsors = relSponsor;
            };

            return View(detailsVM);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int Id)
        {
            string token = RetrieveTokenFromSession();

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PassSessionUsername();

            try
            {
                HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/{API_ENDPOINT}/{Id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return View();
            }

            return View();
        }

        private async Task<HttpResponseMessage> SendListingToApi(EvHeaderCreateDTO listingDTO)
        {
            using (var httpClient = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Headers.ContentType.MediaType = "multipart/form-data";

                    formData.Add(new StringContent(listingDTO.EventName), "EventName");
                    formData.Add(new StringContent(listingDTO.EventLocation), "EventLocation");
                    formData.Add(new StringContent(listingDTO.EventDate.ToString()), "EventDate");
                    formData.Add(new StringContent(listingDTO.EventDesc), "EventDesc");
                    formData.Add(new StringContent(listingDTO.SelectedCategoryId.ToString()), "CategoryId");

                    // Add the file to the request
                    if (listingDTO.PromoImage != null && listingDTO.PromoImage.Length > 0)
                    {
                        formData.Add(new StreamContent(listingDTO.PromoImage.OpenReadStream())
                        {
                            Headers = { ContentLength = listingDTO.PromoImage.Length,
                                            ContentType = new MediaTypeHeaderValue(
                                                listingDTO.PromoImage.ContentType)
                                        }

                        }, "PromoImage", listingDTO.PromoImage.FileName);
                    }

                    if (listingDTO.PromoImage2 != null && listingDTO.PromoImage2.Length > 0)
                    {
                        formData.Add(new StreamContent(listingDTO.PromoImage2.OpenReadStream())
                        {
                            Headers = { ContentLength = listingDTO.PromoImage2.Length,
                                            ContentType = new MediaTypeHeaderValue(
                                                listingDTO.PromoImage2.ContentType)
                                        }

                        }, "PromoImage2", listingDTO.PromoImage2.FileName);
                    }

                    if (listingDTO.PromoImage3 != null && listingDTO.PromoImage3.Length > 0)
                    {
                        formData.Add(new StreamContent(listingDTO.PromoImage3.OpenReadStream())
                        {
                            Headers = { ContentLength = listingDTO.PromoImage3.Length,
                                            ContentType = new MediaTypeHeaderValue(
                                                listingDTO.PromoImage3.ContentType)
                                        }

                        }, "PromoImage3", listingDTO.PromoImage3.FileName);
                    }

                    if (listingDTO.Permit != null && listingDTO.Permit.Length > 0)
                    {
                        formData.Add(new StreamContent(listingDTO.Permit.OpenReadStream())
                        {
                            Headers = { ContentLength = listingDTO.Permit.Length,
                                            ContentType = new MediaTypeHeaderValue(
                                                listingDTO.Permit.ContentType)
                                        }

                        }, "Permit", listingDTO.Permit.FileName);
                    }

                    // Send to API Code
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

                    // Send the data to the API
                    return await httpClient.PostAsync($"{_client.BaseAddress}/{API_ENDPOINT}", formData);
                }
            }
        }

        private async Task<List<HttpResponseMessage>> SendAdmissionToApi(EvHeaderCreateDTO listingDTO, int eventId)
        {
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();

            foreach (var admission in listingDTO.Admission)
            {
                var aList = new Admission()
                {
                    Id = 0,
                    AdmissionType = admission.AdmissionType,
                    Price = admission.Price,
                    Quantity = admission.Quantity,
                    IsDeleted = false,
                    IsNotPurchasable = admission.IsNotPurchasable,
                    EventListingId = eventId,
                };

                var aData = JsonConvert.SerializeObject(aList);
                StringContent aContent = new StringContent(aData, System.Text.Encoding.UTF8, "application/json");
                var aResponse = _client.PostAsync($"{_client.BaseAddress}/admission", aContent).Result;

                responses.Add(aResponse);
            }

            return responses;
        }

        private async Task<List<HttpResponseMessage>> SendMusicToApi(EvHeaderCreateDTO listingDTO, int eventId)
        {
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();

            foreach (var music in listingDTO.Music)
            {
                var mList = new Music()
                {
                    Id = 0,
                    MusicProvider = music.MusicProvider,
                    IsDeleted = false,
                    EventListingId = eventId,
                };

                var mData = JsonConvert.SerializeObject(mList);
                StringContent mContent = new StringContent(mData, System.Text.Encoding.UTF8, "application/json");
                var mResponse = _client.PostAsync($"{_client.BaseAddress}/music", mContent).Result;

                responses.Add(mResponse);
            }
            return responses;
        }

        private async Task<List<HttpResponseMessage>> SendOutletToApi(EvHeaderCreateDTO listingDTO, int eventId)
        {
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();

            foreach (var outlet in listingDTO.Outlet)
            {
                var oList = new Outlet()
                {
                    Id = 0,
                    OutletName = outlet.OutletName,
                    IsDeleted = false,
                    EventListingId = eventId,

                };

                var oData = JsonConvert.SerializeObject(oList);
                StringContent oContent = new StringContent(oData, System.Text.Encoding.UTF8, "application/json");
                var oResponse = _client.PostAsync($"{_client.BaseAddress}/outlet", oContent).Result;

                responses.Add(oResponse);
            }
            return responses;
        }

        private async Task<List<HttpResponseMessage>> SendSponsorToApi(EvHeaderCreateDTO listingDTO, int eventId)
        {
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();

            foreach (var sponsor in listingDTO.Sponsor)
            {
                var sList = new Sponsor()
                {
                    Id = 0,
                    EventSponsor = sponsor.EventSponsor,
                    IsDeleted = false,
                    EventListingId = eventId,

                };

                var sData = JsonConvert.SerializeObject(sList);
                StringContent sContent = new StringContent(sData, System.Text.Encoding.UTF8, "application/json");
                var sResponse = _client.PostAsync($"{_client.BaseAddress}/sponsor", sContent).Result;

                responses.Add(sResponse);
            }
            return responses;
        }

        private async Task<HttpResponseMessage> UpdateListingToApi(EvHeaderCreateDTO listing)
        {
            using (var httpClient = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Headers.ContentType.MediaType = "multipart/form-data";

                    formData.Add(new StringContent(listing.Id.ToString()), "Id"); //maybe this is what causes the error
                    formData.Add(new StringContent(listing.EventName), "EventName");
                    formData.Add(new StringContent(listing.EventLocation), "EventLocation");
                    formData.Add(new StringContent(listing.EventDate.ToString()), "EventDate");
                    formData.Add(new StringContent(listing.EventDesc), "EventDesc");
                    formData.Add(new StringContent(listing.SelectedCategoryId.ToString()), "CategoryId");

                    //Add the file to the request
                    if (listing.PromoImage != null && listing.PromoImage.Length > 0)
                    {
                        formData.Add(new StreamContent(listing.PromoImage.OpenReadStream())
                        {
                            Headers = { ContentLength = listing.PromoImage.Length,
                                            ContentType = new MediaTypeHeaderValue(
                                                listing.PromoImage.ContentType)
                                        }

                        }, "PromoImage", listing.PromoImage.FileName);
                    }

                    if (listing.PromoImage2 != null && listing.PromoImage2.Length > 0)
                    {
                        formData.Add(new StreamContent(listing.PromoImage2.OpenReadStream())
                        {
                            Headers = { ContentLength = listing.PromoImage2.Length,
                                            ContentType = new MediaTypeHeaderValue(
                                                listing.PromoImage2.ContentType)
                                        }

                        }, "PromoImage2", listing.PromoImage2.FileName);
                    }

                    if (listing.PromoImage3 != null && listing.PromoImage3.Length > 0)
                    {
                        formData.Add(new StreamContent(listing.PromoImage3.OpenReadStream())
                        {
                            Headers = { ContentLength = listing.PromoImage3.Length,
                                            ContentType = new MediaTypeHeaderValue(
                                                listing.PromoImage3.ContentType)
                                        }

                        }, "PromoImage3", listing.PromoImage3.FileName);
                    }

                    if (listing.Permit != null && listing.Permit.Length > 0)
                    {
                        formData.Add(new StreamContent(listing.Permit.OpenReadStream())
                        {
                            Headers = { ContentLength = listing.Permit.Length,
                                            ContentType = new MediaTypeHeaderValue(
                                                listing.Permit.ContentType)
                                        }

                        }, "Permit", listing.Permit.FileName);
                    }

                    // Send to API Code
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

                    // Send the data to the API
                    return await httpClient.PutAsync($"{_client.BaseAddress}/{API_ENDPOINT}/{listing.Id}", formData);
                }
            }
        }

        private async Task<List<HttpResponseMessage>> UpdateAdmissionToApi(EvHeaderCreateDTO listing, int eventId)
        {
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();

            foreach (var admission in listing.Admission)
            {
                var aList = new Admission()
                {
                    Id = admission.Id,
                    AdmissionType = admission.AdmissionType,
                    Price = admission.Price,
                    Quantity = admission.Quantity,
                    IsDeleted = false,
                    IsNotPurchasable = admission.IsNotPurchasable,
                    EventListingId = eventId,
                };

                var aData = JsonConvert.SerializeObject(aList);
                StringContent aContent = new StringContent(aData, System.Text.Encoding.UTF8, "application/json");
                var aResponse = _client.PutAsync($"{_client.BaseAddress}/admission/{admission.Id}", aContent).Result;

                responses.Add(aResponse);
            }

            return responses;
        }

        private async Task<List<HttpResponseMessage>> UpdateMusicToApi(EvHeaderCreateDTO listing, int eventId)
        {
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();

            foreach (var music in listing.Music)
            {
                var mList = new Music()
                {
                    Id = music.Id,
                    MusicProvider = music.MusicProvider,
                    IsDeleted = false,
                    EventListingId = eventId,
                };

                var mData = JsonConvert.SerializeObject(mList);
                StringContent mContent = new StringContent(mData, System.Text.Encoding.UTF8, "application/json");
                var mResponse = _client.PutAsync($"{_client.BaseAddress}/music/{music.Id}", mContent).Result;

                responses.Add(mResponse);
            }
            return responses;
        }

        private async Task<List<HttpResponseMessage>> UpdateOutletToApi(EvHeaderCreateDTO listing, int eventId)
        {
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();

            foreach (var outlet in listing.Outlet)
            {
                var oList = new Outlet()
                {
                    Id = outlet.Id,
                    OutletName = outlet.OutletName,
                    IsDeleted = false,
                    EventListingId = eventId,

                };

                var oData = JsonConvert.SerializeObject(oList);
                StringContent oContent = new StringContent(oData, System.Text.Encoding.UTF8, "application/json");
                var oResponse = _client.PutAsync($"{_client.BaseAddress}/outlet/{outlet.Id}", oContent).Result;

                responses.Add(oResponse);
            }
            return responses;
        }

        private async Task<List<HttpResponseMessage>> UpdateSponsorToApi(EvHeaderCreateDTO listing, int eventId)
        {
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();

            foreach (var sponsor in listing.Sponsor)
            {
                var sList = new Sponsor()
                {
                    Id = sponsor.Id,
                    EventSponsor = sponsor.EventSponsor,
                    IsDeleted = false,
                    EventListingId = eventId,

                };

                var sData = JsonConvert.SerializeObject(sList);
                StringContent sContent = new StringContent(sData, System.Text.Encoding.UTF8, "application/json");
                var sResponse = _client.PutAsync($"{_client.BaseAddress}/sponsor/{sponsor.Id}", sContent).Result;

                responses.Add(sResponse);
            }
            return responses;
        }
    }
}