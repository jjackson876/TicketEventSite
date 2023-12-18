using EventsClient.Models;
using EventsClient.Models.ViewModels;
using EventsClients.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace EventsClient.Controllers
{
    public class BookingController : Controller
    {
        const string API_URL = "https://localhost:7161/api";
        Uri baseAddress2 = new Uri("https://localhost:7161/auth");

        const string EVENT_ENDPOINT = "EventListing";
        const string BOOKING_ENDPOINT = "Booking";
        const string BOUGHT_TICKETS_ENDPOINT = "BoughtTicket";
        const string ADMISSION_ENDPOINT = "Admission";
        const string MUSIC_ENDPOINT = "Music";
        const string OUTLET_ENDPOINT = "Outlet";
        const string SPONSOR_ENDPOINT = "Sponsor";


        Uri baseAddress = new Uri(API_URL);
        private readonly HttpClient _client;
        private readonly HttpClient _client2;

        public BookingController()
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

        // helper method to get data from api
        static List<T> GetData<T>(HttpClient client, string endpoint, Func<T, bool> filter) where T : class
        {
            var response = client.GetAsync($"{client.BaseAddress}/{endpoint}").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var listing = JsonConvert.DeserializeObject<List<T>>(content);
            return listing.Where(filter).ToList();
        }

        static HttpResponseMessage PostData<T>(HttpClient client, string endpoint, T values) where T : class
        {
            var data = JsonConvert.SerializeObject(values);
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            var response = client.PostAsync($"{client.BaseAddress}/{endpoint}", content).Result;
            return response;
        }

        static HttpResponseMessage PutData<T>(HttpClient client, string endpoint, T values, int Id) where T : class
        {
            var data = JsonConvert.SerializeObject(values);
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            var response = client.PutAsync($"{client.BaseAddress}/{endpoint}/{Id}", content).Result;
            return response;
        }

        static HttpResponseMessage DeleteData<T>(HttpClient client, string endpoint, int Id)
        {
            var response = client.DeleteAsync($"{client.BaseAddress}/{endpoint}/{Id}").Result;
            return response;
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

            var bookList = GetData<Booking>(_client, BOOKING_ENDPOINT, x => true);

            // Filter bookings by user ID
            //var bookList = GetData<Booking>(_client, BOOKING_ENDPOINT, x => x.UserId == HttpContext.User.Identity.Name);


            return View(bookList);
        }

        [HttpGet]
        public IActionResult Events()
        {
            var evList = GetData<EvHeader>(_client, EVENT_ENDPOINT, x => true);
            var catList = GetData<Category>(_client, "Category", x => true);

            HomeVM homeVM = new HomeVM();
            {
                homeVM.Events = evList;
                homeVM.Categories = catList;
            };
            return View(homeVM);
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            var eventItem = GetData<EvHeader>(_client, EVENT_ENDPOINT, x => x.Id == Id).FirstOrDefault();
            var eId = eventItem.Id;

            var admissions = GetData<Admission>(_client, ADMISSION_ENDPOINT, x => x.EventListingId == eventItem.Id);
            var musics = GetData<Music>(_client, MUSIC_ENDPOINT, x => x.EventListingId == eventItem.Id);
            var outlets = GetData<Outlet>(_client, OUTLET_ENDPOINT, x => x.EventListingId == eventItem.Id);
            var sponsors = GetData<Sponsor>(_client, SPONSOR_ENDPOINT, x => x.EventListingId == eventItem.Id);

            var viewmodel = new DetailsVM();
            {
                viewmodel.Events = eventItem;
                viewmodel.Admissions = admissions;
                viewmodel.Musics = musics;
                viewmodel.Outlets = outlets;
                viewmodel.Sponsors = sponsors;
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
            var values = GetData<EvHeader>(_client, EVENT_ENDPOINT, x => x.Id == Id).FirstOrDefault();

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

        [HttpGet]
        public IActionResult Create(int Id)
        {
            List<Admission> relAdmission = new List<Admission>();
            List<Music> relMusic = new List<Music>();
            List<Outlet> relOutlet = new List<Outlet>();
            List<Sponsor> relSponsor = new List<Sponsor>();

            // gets a single event
            var singleEvent = GetData<EvHeader>(_client, EVENT_ENDPOINT, x => x.Id == Id).FirstOrDefault();

            if (singleEvent != null)
            {
                var eventId = singleEvent.Id;

                var admissionList = GetData<Admission>(_client, ADMISSION_ENDPOINT, x => true);
                var musicList = GetData<Music>(_client, MUSIC_ENDPOINT, x => true);
                var outletList = GetData<Outlet>(_client, OUTLET_ENDPOINT, x => true);
                var sponsorList = GetData<Sponsor>(_client, SPONSOR_ENDPOINT, x => true);

                foreach (var a in admissionList)
                {
                    if (eventId == a.EventListingId)
                    {
                        relAdmission.Add(a);
                    }
                }

                foreach (var a in musicList)
                {
                    if (eventId == a.EventListingId)
                    {
                        relMusic.Add(a);
                    }
                }

                foreach (var a in relOutlet)
                {
                    if (eventId == a.EventListingId)
                    {
                        outletList.Add(a);
                    }
                }

                foreach (var a in sponsorList)
                {
                    if (eventId == a.EventListingId)
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

            return View();
        }

        [HttpPost]
        public IActionResult Create(Booking booking)
        {
            string token = RetrieveTokenFromSession();

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PassSessionUsername();

            booking.UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //System.Diagnostics.Debugger.Break();

            var response = PostData<Booking>(_client, BOOKING_ENDPOINT, booking);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View(booking);
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

            var booking = GetData<Booking>(_client, BOOKING_ENDPOINT, x => x.Id == Id).FirstOrDefault();
            if (booking != null)
            {
                return View(booking);
            }
            return View();
        }

        [HttpPost]
        public IActionResult Edit(Booking booking)
        {
            string token = RetrieveTokenFromSession();

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            PassSessionUsername();

            var response = PutData<Booking>(_client, BOOKING_ENDPOINT, booking, booking.Id);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View(booking);
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

            var booking = GetData<Booking>(_client, BOOKING_ENDPOINT, x => x.Id == Id).FirstOrDefault();
            if (booking != null)
            {
                return View(booking);
            }
            return View();
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

            // get the booking so you can get the id
            var bookList = GetData<Booking>(_client, BOOKING_ENDPOINT, x => true);
            var bookingItem = bookList.FirstOrDefault(x => x.Id == Id);
            var bookingEventId = bookingItem.EventListingId;



            // get the admissions
            var admissions = GetData<Admission>(_client, ADMISSION_ENDPOINT, x => x.EventListingId == bookingEventId);

            // get all the tickets bought
            var boughtTickets = GetData<BoughtTicket>(_client, "BoughtTicket", x => x.BookingId == Id);

            foreach (var b in boughtTickets)
            {
                var tQuantity = b.Quantity;

                foreach (var a in admissions)
                {
                    var currTQuantity = a.Quantity;

                    var aList = new Admission()
                    {
                        Id = 0,
                        AdmissionType = a.AdmissionType,
                        Price = a.Price,
                        Quantity = currTQuantity + tQuantity,
                        IsDeleted = a.IsDeleted,
                        IsNotPurchasable = a.IsNotPurchasable,
                        EventListingId = a.EventListingId
                    };

                    var aResponse = PutData<Admission>(_client, ADMISSION_ENDPOINT, aList, (int)a.Id);
                }
            }

            var response = DeleteData<Booking>(_client, BOOKING_ENDPOINT, Id);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Summary(int bId)
        {
            var bookingItem = GetData<Booking>(_client, BOOKING_ENDPOINT, x => x.Id == bId).FirstOrDefault();

            if (bookingItem != null)
            {
                var eventItem = GetData<EvHeader>(_client, EVENT_ENDPOINT, x => x.Id == bookingItem.EventListingId).FirstOrDefault();

                if (eventItem != null)
                {
                    var tickets = GetData<BoughtTicket>(_client, BOUGHT_TICKETS_ENDPOINT, x => x.BookingId == bookingItem.Id).ToList();

                    if (tickets != null)
                    {
                        var viewmodel = new SummaryVM();
                        {
                            viewmodel.Events = eventItem;
                            viewmodel.Bookings = bookingItem;
                            viewmodel.ListOfTickets = tickets;
                        };

                        return View(viewmodel);
                    }
                }
            }
            return null;
        }

        [HttpPost, ActionName("Summary")]
        public IActionResult SummaryPost(SummaryVM vm, int bId)
        {
            var bvalues = GetData<Booking>(_client, BOOKING_ENDPOINT, x => x.Id == bId).FirstOrDefault();

            bvalues.FirstName = vm.FirstName;
            bvalues.LastName = vm.LastName;
            bvalues.PhoneNumber = vm.PhoneNumber;
            bvalues.Email = vm.Email;
            bvalues.CCardNumber = vm.CCardNumber;
            bvalues.CCardCVV = vm.CCardCVV;
            bvalues.CCardExpDate = vm.CCardExpDate;

            var bResponse = PutData<Booking>(_client, ADMISSION_ENDPOINT, bvalues, bId);

            if (bResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View(bvalues);
        }

        private async Task<HttpResponseMessage> SendBookingToApi(int listingId)
        {
            var booking = new Booking()
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

            var bResponse = PostData<Booking>(_client, BOOKING_ENDPOINT, booking);

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

            var singleEvent = GetData<EvHeader>(_client, EVENT_ENDPOINT, x => x.Id == eventId).FirstOrDefault();
            var eventID = singleEvent.Id;

            // if the selected event wasnt retrieved successful then it wont enter the block
            if (singleEvent != null)
            {
                // call all admissions 
                var matchAdmissions = GetData<Admission>(_client, ADMISSION_ENDPOINT, x => x.EventListingId == singleEvent.Id);

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

                            var aResponse = PutData<Admission>(_client, ADMISSION_ENDPOINT, matchAdmissions[i], (int)matchAdmissions[i].Id);

                            var tResponse = PostData<BoughtTicket>(_client, "BoughtTicket", tickets);

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

        [HttpPost]
        public IActionResult DeletePartial(int Id)
        {
            var response = DeleteData<Booking>(_client, BOOKING_ENDPOINT, Id);

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
    }
}