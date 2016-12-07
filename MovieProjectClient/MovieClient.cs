using MovieModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace MovieProjectClient
{
    public static class Menu
    {
        public static string FrontEnd()
        {
            Menu.DisplayBox("Cinema Listings App", 11);

            string[] menuMain = new string[]
            {
                "[C] Local Cinema Index",
                "[M] Now Showing",
                "[F] Find Venue",
                "[S] Search By Title",
                "[G] Search By Genre",
                "[X] Quit App"
            };

            foreach (string option in menuMain)
            {
                Console.WriteLine(option);
            }

            // User button input taken as ConsoleKeyInfo object - Key property is always Upper(case) value
            Console.Write("\n\t");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Press a key to select an option: ");
            Console.ResetColor();
            ConsoleKeyInfo choice = Console.ReadKey(true);

            return (choice.Key.ToString());
        }

        public static int NumberInput(List<string> menuTags, int colour, int range)
        {
            if (range == 0)                         // zero flag to print and count options if not already displayed
            {
                range = menuTags.Count();
                for (int i = 0; i < range; i++)
                {
                    Console.WriteLine("[{0}] {1}", i + 1, menuTags[i]);
                }
            }
            Console.Write("\n\t");
            Console.ForegroundColor = (ConsoleColor)colour;
            Console.Write("Press a key to select an option: ");
            Console.ResetColor();

            // remember that return value is an indexer, so option [1] is index [0]
            int numTag = 0;

            do
            {
                ConsoleKeyInfo choice = Console.ReadKey(true);
                if (Char.IsNumber(choice.KeyChar))
                {
                    Int32.TryParse(choice.KeyChar.ToString(), out numTag);     //safer way of parsing number from string
                };
            } while (numTag < 1 || numTag > range);                             // needs to go alphanumeric / hex if over 9 entries TO DO

            return (numTag - 1);
        }

        public static string UserInput(string query)
        {
            string choice = "Go!";
            do
            {
                Console.Write("\n" + query + " ");
                choice = Console.ReadLine();
            } while (choice == "");
            return choice;
        }

        public static void DisplayBox(string title, int colour)
        {
            Console.ForegroundColor = (ConsoleColor)colour;
            Console.Write("_______________________________________\n\n\t");
            Console.WriteLine(title + "\n_______________________________________\n");
            Console.ResetColor();
        }
    }

    class Client
    {
        // GET to list all Cinemas
        static async Task<List<string>> GetAllCinemasAsync()
        {
            List<string> idstring = new List<string>();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");                             // base URL for API Controller i.e. RESTFul service

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Cinemas
                    // get all Cinema venues
                    HttpResponseMessage response = await client.GetAsync("Cinemas/");                  // async call, await suspends until result available            
                    if (response.IsSuccessStatusCode)                                                   // 200..299
                    {
                        // read result
                        Menu.DisplayBox("Local Cinema Index", 11);
                        int i = 0;
                        var venues = await response.Content.ReadAsAsync<IEnumerable<Cinema>>();
                        foreach (var v in venues)
                        {
                            i++;
                            idstring.Add(v.CinemaID);
                            Console.Write("[" + i + "] " + v.Name + "\t " + v.Website + " \t" + v.PhoneNumber + " ");
                            Console.WriteLine(" Main screen: " + v.Movies.Title);
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return idstring;
        }

        // GET to display a single Cinema
        static async Task GetCinemaAsync(string id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");                             // base URL for API Controller i.e. RESTFul service

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Cinemas/id
                    // get a particular Cinema
                    HttpResponseMessage response = await client.GetAsync("Cinemas/" + id);                  // async call, await suspends until result available            
                    if (response.IsSuccessStatusCode)                                                   // 200..299
                    {
                        // read result
                        Menu.DisplayBox("Cinema Details", 11);

                        var v = await response.Content.ReadAsAsync<Cinema>();

                        Console.WriteLine("Cinema:  " + v.Name);
                        Console.WriteLine("Online booking at  " + v.Website);
                        Console.WriteLine("Box Office Tel. " + v.PhoneNumber);
                        Console.WriteLine("Standard Ticket Price:  " + v.TicketPrice);
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("Main Screen:  " + v.Movies.Title);
                        Console.ResetColor();

                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //GET to find a Cinema id by name or part of name
        static async Task GetCinemasBySearchAsync(string id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");                             // base URL for API Controller i.e. RESTFul service

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Cinemas
                    // get all venues
                    HttpResponseMessage response = await client.GetAsync("Cinemas/Search/" + id);                  // async call, await suspends until result available            
                    if (response.IsSuccessStatusCode)                                                   // 200..299
                    {
                        // read result into iterable IEnumerable
                        Menu.DisplayBox("Cinema Search", 6);
                        var venues = await response.Content.ReadAsAsync<IEnumerable<Cinema>>();

                        if (venues.Count() != 0)
                        {

                            foreach (var v in venues)
                            {

                                Console.WriteLine("Cinema: " + v.Name);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No match found");
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // GET to list all Movies
        static async Task<List<string>> GetAllMoviesAsync()
        {
            List<string> idstring = new List<string>();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");                             // base URL for API Controller i.e. RESTFul service

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Movies
                    // get all movie screenings
                    HttpResponseMessage response = await client.GetAsync("Movies/");                  // async call, await suspends until result available            
                    if (response.IsSuccessStatusCode)                                                   // 200..299
                    {
                        // read result into iterable IEnumerable
                        Menu.DisplayBox("Movies Showing This Week", 6);
                        int i = 0;
                        var screenings = await response.Content.ReadAsAsync<IEnumerable<Movie>>();
                        foreach (var s in screenings)
                        {
                            idstring.Add(s.MovieID);
                            i++;
                            string cert = s.Certification.ToString().Substring("IFCO".Length);
                            string showtime = s.ShowTime.ToString().Remove(5);
                            Console.Write("[" + i + "] " + s.Title + "\tRating: " + cert + " \tNext screening " + showtime + "\n");
                            //Console.WriteLine("\t Now showing at {0} Cinema{1}", s.Cinemas.Count, (s.Cinemas.Count == 1 ? "" : "s"));
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return idstring;
        }

        // GET to list all Cinemas where a Movie is Showing
        static async Task GetMovieScreenings(string id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");                             // base URL for API Controller i.e. RESTFul service

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Movies/Screenings/id
                    // get single movie screenings
                    HttpResponseMessage response = await client.GetAsync("Movies/Screenings/" + id);                  // async call, await suspends until result available            
                    if (response.IsSuccessStatusCode)                                                   // 200..299
                    {
                        // read result into iterable IEnumerable

                        Console.WriteLine("\nThis movie is currently showing at ");
                        var venues = await response.Content.ReadAsAsync<IEnumerable<Cinema>>();
                        foreach (var v in venues)
                        {
                            Console.WriteLine(v.Name + " - book online at " + v.Website + ". Phone booking:" + v.PhoneNumber + ". Tickets " + v.TicketPrice);
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // GET to display a single Movie
        static async Task GetMovieAsync(string id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");                             // base URL for API Controller locally *adjust for Azure*

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Movies/id
                    // get a particular Movie
                    HttpResponseMessage response = await client.GetAsync("Movies/" + id);                  // async call, await suspends until result available            
                    if (response.IsSuccessStatusCode)                                                   // 200..299
                    {
                        // read result
                        var s = await response.Content.ReadAsAsync<Movie>();
                        Menu.DisplayBox(s.Title, 13);
                        string cert = "Certificate: " + s.Certification.ToString().Substring("IFCO".Length);
                        string showtime = s.ShowTime.ToString().Remove(5);
                        Console.WriteLine(cert + "   Genre: " + s.Genre.ToString());
                        Console.WriteLine("\n" + s.Description + " \n");
                        //Console.Write("Now showing at {0} Cinema{1}. ", s.Cinemas.Count, (s.Cinemas.Count == 1 ? "" : "s"));
                        Console.WriteLine("Program starts " + showtime + ". Running Time: " + s.RunTime + " mins.");
                        Console.WriteLine("Next performance " + s.MovieNow(s.ShowTime));
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // call screenings before returning
            GetMovieScreenings(id).Wait();
        }

        // GET to find a movie by genre
        static async Task GetMoviesByGenreAsync(int id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");                             // base URL for API Controller *CHANGE FOR AZURE"

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string g = id.ToString();
                    HttpResponseMessage response = await client.GetAsync("Movies/Genre/" + g);
                    if (response.IsSuccessStatusCode)                                                   // 200..299
                    {
                        // read result into iterable IEnumerable
                        Menu.DisplayBox("Movies by Genre: " + Enum.GetName(typeof(Genre), id), 6);                        // also =Enum.GetName(typeof(Genre), g) or 
                        var gens = await response.Content.ReadAsAsync<IEnumerable<Movie>>();
                        foreach (var s in gens)
                        {
                            string cert = s.Certification.ToString().Substring("IFCO".Length);
                            string showtime = s.ShowTime.ToString().Remove(5);
                            Console.Write("Movie: " + s.Title + " " + cert + " " + showtime + " ");
                            //Console.WriteLine(" Now showing at {0} Cinema{1}", s.Cinemas.Count, (s.Cinemas.Count == 1 ? "" : "s"));
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //Get movies by search term on title string
        static async Task GetMoviesBySearchTermAsync(string id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");                             // base URL for API Controller i.e. RESTFul service

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Movies
                    // get all movie screenings
                    HttpResponseMessage response = await client.GetAsync("Movies/Titlesearch/" + id);                  // async call, await suspends until result available            
                    if (response.IsSuccessStatusCode)                                                   // 200..299
                    {
                        // read result into iterable IEnumerable
                        Menu.DisplayBox("Movie Search", 6);
                        var screenings = await response.Content.ReadAsAsync<IEnumerable<Movie>>();

                        if (screenings.Count() != 0)
                        {

                            foreach (var s in screenings)
                            {
                                string cert = s.Certification.ToString().Substring("IFCO".Length);
                                string showtime = s.ShowTime.ToString().Remove(5);
                                Console.Write("Movie: " + s.Title + "\tRating: " + cert + " \tNext screening " + showtime);
                                //Console.WriteLine("\t Now showing at {0} Cinema{1}", s.Cinemas.Count, (s.Cinemas.Count == 1 ? "" : "s"));
                            }
                        }
                        else
                        {
                            Console.WriteLine("No match found");
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        static void Main()
        {
            Console.Title = "EAD PROJECT - ISZN, The Movie Listings App";

            bool IsUsing = true;
            do
            {
                string input = Menu.FrontEnd();
                RunAsync(input).Wait();
                if (input == "X")
                {
                    IsUsing = false;
                }
            } while (IsUsing);
        }

        static async Task RunAsync(string path)
        {
            switch (path)
            {
                case "C":
                    Console.Clear();
                    List<string> allVenues = await GetAllCinemasAsync();
                    int venue = Menu.NumberInput(allVenues, 11, allVenues.Count);
                    Console.Clear();
                    GetCinemaAsync(allVenues[venue]).Wait();
                    break;

                case "M":
                    Console.Clear();
                    List<string> allMovies = await GetAllMoviesAsync();
                    int show = Menu.NumberInput(allMovies, 6, allMovies.Count);
                    Console.Clear();
                    GetMovieAsync(allMovies[show]).Wait();
                    break;

                case "F":
                    Console.Clear();
                    string cSearch = Menu.UserInput("Please enter a search term for the cinema:");
                    GetCinemasBySearchAsync(cSearch).Wait();
                    break;

                case "S":
                    Console.Clear();
                    string mSearch = Menu.UserInput("Please enter a search term for the movie title:");
                    GetMoviesBySearchTermAsync(mSearch).Wait();
                    break;

                case "G":
                    Console.Clear();
                    Menu.DisplayBox("Movies by genre", 6);
                    List<string> genres = new List<string> { "Horror", "Comedy", "Fantasy", "Action", "Family", "Romance" };
                    int gSearch = Menu.NumberInput(genres, 6, 0);
                    Console.WriteLine("\n\n");
                    GetMoviesByGenreAsync(gSearch).Wait();
                    break;

                case "X":
                    Console.Clear();
                    Console.WriteLine("Quitting application...");
                    break;
            }
            Console.ReadLine();
        }
    }
}
