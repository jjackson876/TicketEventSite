using EventsClient.Models;
using EventsClient.Models.DTOs;
using EventsClient.Models.ViewModels;
using EventsClient.Utils;
using EventsClients.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace EventsClient.Controllers
{
    public class HomeController : Controller
    {
        const string API_URL = "https://localhost:7161/api";
        const string AUTH_URL = "https://localhost:7161/auth";

        Uri baseAddress = new Uri(API_URL);
        Uri baseAddress2 = new Uri(AUTH_URL);

        const string API_ENDPOINT = "EventListing";
        const string BOOKING_ENDPOINT = "Booking";
        const string BOUGHT_TICKETS_ENDPOINT = "BoughtTicket";


        private readonly HttpClient _client;
        private readonly HttpClient _client2;

        private readonly ILogger<HomeController> _logger;
        //private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger)  /*, UserManager<IdentityUser> userManager */
        {
            _logger = logger;
            //_userManager = userManager;

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


        static async Task<List<T>> GetData<T>(HttpClient client, string endpoint, Func<T, bool> filter) where T : class
        {
            var response = await client.GetAsync($"{client.BaseAddress}/{endpoint}");
            var content = await response.Content.ReadAsStringAsync();
            var listing = JsonConvert.DeserializeObject<List<T>>(content);
            return listing.Where(filter).ToList();
        }


        static async Task<HttpResponseMessage> DeleteData<T>(HttpClient client, string endpoint, int Id)
        {
            var response = await client.DeleteAsync($"{client.BaseAddress}/{endpoint}/{Id}");
            return response;
        }


        public async Task<IActionResult> Index(string term, int currentPage = 1)
        {
            // task run is an async workaround -> running a sync task on a separate thread  
            await Task.Run(() => DeleteEmptyBookings());

            var evList = await Task.Run(() => GetData<EvHeader>(_client, API_ENDPOINT, x => true));
            var catList = await Task.Run(() => GetData<Category>(_client, "Category", x => true));

            var totalRecords = evList.Count();
            var pageSize = 3;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var obj = new HomeVM
            {
                Data = evList.AsQueryable(),
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = totalPages,
                Term = term,
                Categories = catList,
            };

            if (String.IsNullOrEmpty(term))
            {
                obj.Data = evList.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList().AsQueryable();
                return View(obj);
            }
            else
            {
                term = term.ToLower();

                var searchedItems = obj.Data.Where(s =>
                s.EventName != null && s.EventName.ToLower().Contains(term) ||
                s.Category != null && s.Category.CategoryName != null &&
                s.Category.CategoryName.ToLower().Contains(term)).ToList();

                var totalRecords2 = searchedItems.Count();
                obj.TotalPages = (int)Math.Ceiling(totalRecords2 / (double)obj.PageSize);
                obj.CurrentPage = currentPage;

                // Apply pagination to the filtered results
                searchedItems = searchedItems.Skip((currentPage - 1) * obj.PageSize).Take(obj.PageSize).ToList();

                obj.Data = searchedItems.AsQueryable();

                return View(obj);
            }
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
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

            var viewmodel = new DetailsVM();
            {
                viewmodel.Events = singleEvent;
                viewmodel.Admissions = relAdmission;
                viewmodel.Musics = relMusic;
                viewmodel.Outlets = relOutlet;
                viewmodel.Sponsors = relSponsor;
                viewmodel.BoughtTickets = new BoughtTicket();
            };

            return View(viewmodel);
        }

        [HttpPost, ActionName("Details")]
        public async Task<IActionResult> DetailsPost(DetailsVM vm, int Id)
        {
            // sends booking to the api
            var bResponse = await SendBookingToApi(Id);

            // requests data for event selected
            var eResponse = _client.GetAsync($"{_client.BaseAddress}/{API_ENDPOINT}").Result;
            var eData = eResponse.Content.ReadAsStringAsync().Result;
            var eLists = JsonConvert.DeserializeObject<List<EvHeader>>(eData)!;
            var values = eLists.FirstOrDefault(s => s.Id == Id);

            if (bResponse.IsSuccessStatusCode)
            {
                // get the booking id from the api and stores it in a variable
                var bContent = await bResponse.Content.ReadAsStringAsync();
                var booking = JsonConvert.DeserializeObject<Booking>(bContent);
                var bookingId = booking.Id;

                // sends tickets to their api
                // needs booking to be created first to add tickets after
                var tReponse = await SendBTicketsToApi(vm, Id, bookingId);

                return RedirectToAction("Summary", new { bId = bookingId });
            }
            return View(values);
        }

        private async Task<HttpResponseMessage> SendBookingToApi(int listingId)
        {
            var book = new Booking()
            {
                Id = 0,
                EventListingId = listingId,
                FirstName = "",
                LastName = "",
                Email = "",
                PhoneNumber = "",
                CCardNumber = "",
                CCardExpDate = "",
                CCardCVV = ""
            };

            var bData = JsonConvert.SerializeObject(book);
            StringContent bContent = new StringContent(bData, System.Text.Encoding.UTF8, "application/json");
            var bResponse = _client.PostAsync($"{_client.BaseAddress}/booking", bContent).Result;

            if (bResponse.IsSuccessStatusCode)
            {
                return bResponse;
            }
            return null;
        }

        private async Task<List<HttpResponseMessage>> SendBTicketsToApi(DetailsVM vm, int eventId, int bookingId)
        {
            // declare the variable for responses
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();

            // call all responses from api & save the id for the event thats allocated to the selected event
            var eResponse = await _client.GetAsync($"{_client.BaseAddress}/{API_ENDPOINT}");
            var eData = eResponse.Content.ReadAsStringAsync().Result;
            var eLists = JsonConvert.DeserializeObject<List<EvHeader>>(eData)!;
            var singleEvent = eLists.FirstOrDefault(s => s.Id == eventId);
            var eventID = singleEvent.Id;

            // if the selected event wasnt retrieved successful then it wont enter the block
            if (singleEvent != null)
            {
                // call all admissions 
                var aResponse = _client.GetAsync($"{_client.BaseAddress}/admission").Result;
                var aData = aResponse.Content.ReadAsStringAsync().Result;
                var aList = JsonConvert.DeserializeObject<List<Admission>>(aData);

                //declare variable for list of matching admissions 
                List<Admission> matchAdmissions = new List<Admission>();

                // loop thru all the admissions in api
                foreach (var a in aList)
                {
                    // if the eventlisting ID of any of the admissions matches the selected event's ID
                    // then add that admission to the matching admissions list
                    if (singleEvent.Id == a.EventListingId)
                    {
                        matchAdmissions.Add(a);
                    }
                }

                // for loop for to keep track of the admissions added to matchadmission variable 
                for (int i = 0; i < matchAdmissions.Count; i++)
                {
                    try
                    {
                        if (matchAdmissions[i] != null && matchAdmissions[i].AdmissionType != null && matchAdmissions[i].Quantity.HasValue &&
                        vm.TicketAmount != null && vm.TicketAmount.Count >= i && vm.TicketAmount[i] != null && vm.TicketAmount[i] > 0)
                        {
                            var tickets = new BoughtTicket()
                            {
                                Id = 0,
                                TicketType = matchAdmissions[i].AdmissionType, // matches the index of admission displayed to whats in the db 
                                Quantity = vm.TicketAmount[i], // should save the data entered from the view
                                SubTotal = FindSubTotal((int)matchAdmissions[i].Price, vm.TicketAmount[i]),
                                BookingId = bookingId, //could be eItem.Id too 
                            };

                            matchAdmissions[i].Quantity = matchAdmissions[i].Quantity - tickets.Quantity;

                            var aData2 = JsonConvert.SerializeObject(matchAdmissions[i]);
                            var aContent2 = new StringContent(aData2, System.Text.Encoding.UTF8, "application/json");
                            var aResponse2 = _client.PutAsync($"{_client.BaseAddress}/admission/{matchAdmissions[i].Id}", aContent2).Result;

                            var tData = JsonConvert.SerializeObject(tickets);
                            StringContent tContent = new StringContent(tData, System.Text.Encoding.UTF8, "application/json");
                            var tResponse = _client.PostAsync($"{_client.BaseAddress}/boughtticket", tContent).Result;

                            responses.Add(tResponse); // add the response to responses list
                        }
                        else
                        {

                        }
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {

                    }


                }
                return responses; // return the list of responses once the loop is finished
            }
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Summary(int bId)
        {
            var bookingResponse = await _client.GetAsync($"{_client.BaseAddress}/{BOOKING_ENDPOINT}");

            if (bookingResponse.IsSuccessStatusCode)
            {
                var bookingContent = bookingResponse.Content.ReadAsStringAsync().Result;
                var bookingList = JsonConvert.DeserializeObject<List<Booking>>(bookingContent);

                var bookingItem = bookingList.FirstOrDefault(x => x.Id == bId);

                var eventResponse = await _client.GetAsync($"{_client.BaseAddress}/{API_ENDPOINT}");

                if (eventResponse.IsSuccessStatusCode)
                {
                    var eventContent = eventResponse.Content.ReadAsStringAsync().Result;
                    var eventList = JsonConvert.DeserializeObject<List<EvHeader>>(eventContent);

                    var eventItem = eventList.FirstOrDefault(x => x.Id == bookingItem.EventListingId);

                    var ticketResponse = await _client.GetAsync($"{_client.BaseAddress}/{BOUGHT_TICKETS_ENDPOINT}");

                    if (ticketResponse.IsSuccessStatusCode)
                    {
                        var ticketContent = ticketResponse.Content.ReadAsStringAsync().Result;
                        var ticketList = JsonConvert.DeserializeObject<List<BoughtTicket>>(ticketContent);

                        //var ticketItem = ticketList.FirstOrDefault(x => x.BookingId == bookingItem.Id);
                        var ticketItems = ticketList.Where(x => x.BookingId == bookingItem.Id).ToList();

                        var viewmodel = new SummaryVM();
                        {
                            viewmodel.Events = eventItem;
                            viewmodel.Bookings = bookingItem;
                            viewmodel.ListOfTickets = ticketItems;
                        };

                        return View(viewmodel);
                    }
                }
            }
            return null;
        }

        [HttpPost, ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(SummaryVM vm, int bId)
        {
            string token = RetrieveTokenFromSession();

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PassSessionUsername();

            var bListResponse = _client.GetAsync($"{_client.BaseAddress}/{BOOKING_ENDPOINT}").Result;
            var bListData = bListResponse.Content.ReadAsStringAsync().Result;
            var bList = JsonConvert.DeserializeObject<List<Booking>>(bListData)!;
            var bvalues = bList.FirstOrDefault(s => s.Id == bId);

            string userId = "";

            var userResponse = _client.GetAsync($"{_client.BaseAddress}/auth/getuserid").Result;

            if (userResponse.Content.Headers.ContentType.MediaType == "application/json")
            {

                if (userResponse.IsSuccessStatusCode)
                {
                    var userJson = await userResponse.Content.ReadAsStringAsync();
                    var userInfo = JsonConvert.DeserializeAnonymousType(userJson, new { UserId = "" });

                    userId = userInfo.UserId;
                }
                else
                {
                    userId = "is not success status coded";
                }
            }
            else
            {
                userId = "is not json";
            }


            bvalues.UserId = userId;
            bvalues.FirstName = vm.FirstName;
            bvalues.LastName = vm.LastName;
            bvalues.PhoneNumber = vm.PhoneNumber;
            bvalues.Email = vm.Email;
            bvalues.CCardNumber = vm.CCardNumber;
            bvalues.CCardCVV = vm.CCardCVV;
            bvalues.CCardExpDate = vm.CCardExpDate;

            var bData = JsonConvert.SerializeObject(bvalues);
            StringContent bContent = new StringContent(bData, System.Text.Encoding.UTF8, "application/json");
            var bResponse = _client.PutAsync($"{_client.BaseAddress}/{BOOKING_ENDPOINT}/{bId}", bContent).Result;

            if (bResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View("bvalues");
        }

        public IActionResult Profile()
        {
            string token = RetrieveTokenFromSession();

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Login");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PassSessionUsername();

            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePartial(int Id)
        {
            var response = await DeleteData<Booking>(_client, BOOKING_ENDPOINT, Id);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        private double FindSubTotal(int ticketPrice, int ticketQuantity)
        {
            var subtotal = ticketPrice * ticketQuantity;

            return subtotal;
        }

        private async Task DeleteEmptyBookings()
        {
            var AllBookings = await GetData<Booking>(_client, BOOKING_ENDPOINT, x => true);

            for (int i = 0; i < AllBookings.Count; i++)
            {
                if (AllBookings[i].FirstName == null || AllBookings[i].FirstName == ""
                    && AllBookings[i].LastName == null || AllBookings[i].LastName == ""
                    && AllBookings[i].PhoneNumber == null || AllBookings[i].PhoneNumber == ""
                    && AllBookings[i].Email == null || AllBookings[i].Email == ""
                    && AllBookings[i].CCardNumber == null || AllBookings[i].CCardNumber == ""
                    && AllBookings[i].CCardExpDate == null || AllBookings[i].CCardExpDate == ""
                    && AllBookings[i].CCardCVV == null || AllBookings[i].CCardCVV == "")
                {
                    var delete = await DeleteData<Booking>(_client, BOOKING_ENDPOINT, AllBookings[i].Id);
                }
            }
        }
    }
}