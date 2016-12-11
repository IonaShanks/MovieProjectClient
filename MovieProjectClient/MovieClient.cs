using MovieModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace MovieProjectClient
{
    public static class Menu
    {
        const int MAXKEY = 9;

        public static string FrontEnd()
        {
            DisplayBox("Cinema Listings App", 11);

            List<string> menuMain = new List<string>()
            {
                "[C] Local Cinema Index",
                "[M] Now Showing",
                "[F] Find Venue",
                "[S] Search By Title",
                "[G] Search By Genre",
                "[X] Quit App"
            };

            DisplayOptions(menuMain);

            return (UserKeyInput("CMFSGX", 11));
        }

        public delegate int UserInputMethod(int range);

        public static int UserInputReturn(int range, UserInputMethod keyb)
        {
            return keyb(range);
        }

        public static int UserInputPrompt(int range, int colour)
        {
            Console.ForegroundColor = (ConsoleColor)colour;
            int choice = 0;
            if (range > MAXKEY)
            {
                choice = UserInputReturn(range, InputLine);
            }
            else
            {
                choice = UserInputReturn(range, InputKey);
            }
            Console.ResetColor();
            return choice;
        }


        // Delegate method 1 - only works for integers between 0 & 10 exclusive
        public static int InputKey(int range)
        {
            Console.Write("\n\tPress a key to select an option: ");
            // remember that return value is an indexer, so option [1] is index [0]
            int num = 0;

            do
            {
                ConsoleKeyInfo choice = Console.ReadKey(true);
                if (Char.IsNumber(choice.KeyChar))
                {
                    Int32.TryParse(choice.KeyChar.ToString(), out num);     //parsing number from string
                };
            } while (num < 1 || num > range);
            Console.WriteLine(num);
            return (num - 1);
        }

        // Delegate method 2 - for number inputs over MAXPERPAGE
        public static int InputLine(int range)
        {
            // remember that return value is an indexer, so option [1] is index [0]
            int num = 0;
            bool isNum;
            Console.Write("\n\tType the option number and hit enter: ");
            int origRow = Console.CursorTop;
            int origCol = Console.CursorLeft;

            do
            {
                string choice = Console.ReadLine();
                isNum = Int32.TryParse(choice, out num);                        //safer way of parsing number from string
                Console.SetCursorPosition(origCol, origRow);
                Console.Write(new string(' ', choice.Length));
                Console.SetCursorPosition(origCol, origRow);
            } while ((num < 1 || num > range) || !isNum);

            Console.WriteLine(num);
            return (num - 1);
        }

        public static string UserKeyInput(string range, int colour)
        {
            // User button input taken as ConsoleKeyInfo object - letter Key property is always Upper(case) value
            Console.ForegroundColor = (ConsoleColor)colour;
            Console.Write("\n\tPress a key to select an option: ");
            string choice = "";
            do
            {
                ConsoleKeyInfo press = Console.ReadKey(true);
                if (range.Contains(press.Key.ToString()))
                {
                    choice = press.Key.ToString();
                }
            } while (choice == "");
            Console.ResetColor();
            Console.WriteLine(choice);
            return (choice);
        }

        public static string UserTextInput(string query, int colour)
        {
            string choice = "";
            Console.ForegroundColor = (ConsoleColor)colour;
            Console.Write("\n {0}", query);
            int origRow = Console.CursorTop;
            int origCol = Console.CursorLeft;
            do
            {
                choice = Console.ReadLine();
                Console.SetCursorPosition(origCol, origRow);
            } while (choice == "");
            Console.ResetColor();
            Console.WriteLine(choice + "\n");
            return choice;
        }

        public static void DisplayBox(string title, int colour)
        {
            Console.ForegroundColor = (ConsoleColor)colour;
            Console.Write("_______________________________________\n\n\t");
            Console.WriteLine(title + "\n_______________________________________\n");
            Console.ResetColor();
        }

        public static void DisplayOptions(List<string> entries)
        {
            {
                int range = entries.Count();
                for (int i = 0; i < range; i++)
                {
                    if (entries[i][0].ToString() == "[")                     // checks if List already has numbering / letters
                    {
                        Console.WriteLine(entries[i]);
                    }
                    else
                    {
                        Console.WriteLine("[{0}] {1}", i + 1, entries[i]);
                    }

                }
            }
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
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");   // change for connection string

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Cinemas - annotation in controller shorthands route to ../Cinemas/
                    // get all Cinema venues
                    HttpResponseMessage response = await client.GetAsync("Cinemas/");                  //  await suspends processing until async call result available (tends to hang on first query / connection)           
                    if (response.IsSuccessStatusCode)                                                   // are we getting an HTTP return code of 200, 201 etc?
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
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Cinemas/id shorthand to /Cinemas/id
                    // get a particular Cinema
                    HttpResponseMessage response = await client.GetAsync("Cinemas/" + id);                  // will await here            
                    if (response.IsSuccessStatusCode)                                                   // 200-299 is true
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
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Cinemas
                    // get all venues
                    HttpResponseMessage response = await client.GetAsync("Cinemas/Search/" + id);                  // await search results before proceeding           
                    if (response.IsSuccessStatusCode)                                                   // 200-299 means HTTP get was success
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
                            Console.WriteLine("No match found");                                        // HTTP get was success, so empty list returned = no  match
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " " + response.ReasonPhrase);           // HTTP get failed, so don't try to write null list 
                    }
                }
            }
            catch (Exception e)                                                                         // some other error thrown we need to know
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
                    HttpResponseMessage response = await client.GetAsync("Movies/");                  // async return awaited         
                    if (response.IsSuccessStatusCode)                                                   // 200-299 = true
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
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Movies/Screenings/id shorthanded to remove api/
                    // get single movie screenings
                    HttpResponseMessage response = await client.GetAsync("Movies/Screenings/" + id);                  // async call, await result            
                    if (response.IsSuccessStatusCode)                                                   // 200 - 299 = true
                    {
                        // read result into iterable IEnumerable
                        Console.WriteLine("\nThis movie is currently showing at...");
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
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Movies/id shorthanded to remove /api in annotation
                    // get a particular Movie
                    HttpResponseMessage response = await client.GetAsync("Movies/" + id);                  // hangs here until response            
                    if (response.IsSuccessStatusCode)                                                   // 200-299?
                    {
                        // read result
                        var s = await response.Content.ReadAsAsync<Movie>();
                        Menu.DisplayBox(s.Title, 11);
                        string cert = "Certificate: " + s.Certification.ToString().Substring("IFCO".Length);    // remove 'IFCO' from enum name
                        string showtime = s.ShowTime.ToString().Remove(5);                                      // quick and dirty fudge to format a TimeSpan type to first 5 chars
                        Console.WriteLine(cert + "   Genre: " + s.Genre.ToString());
                        Console.WriteLine("\n" + s.Description + " \n");
                        // NB following line worked on localhost but not on Azure (JSIgnore issue)
                        //Console.Write("Now showing at {0} Cinema{1}. ", s.Cinemas.Count, (s.Cinemas.Count == 1 ? "" : "s"));
                        Console.WriteLine("Program starts " + showtime + ". Running Time: " + s.RunTime + " mins.\n");
                        string next  = s.MovieNow(s.ShowTime);
                        Console.WriteLine(next.Remove(next.Length - 3));                                           // another lamentable fudge
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
            Console.ForegroundColor = ConsoleColor.DarkCyan;            // hardcoding bad, ruins code metrics, consider parameter pass / structure
            GetMovieScreenings(id).Wait();
            Console.ResetColor();
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
                    HttpResponseMessage response = await client.GetAsync("Movies/Genre/" + g);          // have to turn enum to str to place in URI
                    if (response.IsSuccessStatusCode)                                                   // 200-299 true
                    {
                        // read result into iterable IEnumerable
                        Menu.DisplayBox("Movies by Genre: " + Enum.GetName(typeof(Genre), id), 6);                        // also =Enum.GetName(typeof(Genre), g) instead of ToString()
                        var gens = await response.Content.ReadAsAsync<IEnumerable<Movie>>();
                        foreach (var s in gens)
                        {
                            string cert = s.Certification.ToString().Substring("IFCO".Length);
                            string showtime = s.ShowTime.ToString().Remove(5);
                            Console.WriteLine("Movie: " + s.Title + " " + cert + " " + showtime + " ");
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
                    client.BaseAddress = new Uri("https://microsoft-apiappd00b15bde6604da799ccb333b729c3a2.azurewebsites.net/");

                    // add an Accept header for JSON
                    client.DefaultRequestHeaders.
                        Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // GET ../api/Movies/TitleSearch/{id]  
                    HttpResponseMessage response = await client.GetAsync("Movies/Titlesearch/" + id);                  // async call           
                    if (response.IsSuccessStatusCode)                                                                 // 200-299 true
                    {
                        // read result into iterable IEnumerable
                        Menu.DisplayBox("Movie Search for '" + id + "'", 6);
                        var screenings = await response.Content.ReadAsAsync<IEnumerable<Movie>>();

                        if (screenings.Count() != 0)
                        {

                            foreach (var s in screenings)
                            {
                                string cert = s.Certification.ToString().Substring("IFCO".Length);
                                string showtime = s.ShowTime.ToString().Remove(5);
                                Console.WriteLine("Movie: " + s.Title + "\tRating: " + cert + " \tNext screening " + showtime);
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
                string choice = Menu.FrontEnd();
                RunAsync(choice).Wait();
                if (choice == "X")
                {
                    IsUsing = false;
                }
            } while (IsUsing);
        }

        static async Task RunAsync(string path)
        {
            Console.Clear();

            switch (path)
            {
                case "C":
                    List<string> allVenues = await GetAllCinemasAsync();
                    int venue = Menu.UserInputPrompt(allVenues.Count, 11);
                    Console.Clear();
                    GetCinemaAsync(allVenues[venue]).Wait();
                    break;

                case "M":
                    List<string> allMovies = await GetAllMoviesAsync();
                    int show = Menu.UserInputPrompt(allMovies.Count, 6);
                    Console.Clear();
                    GetMovieAsync(allMovies[show]).Wait();
                    break;

                case "F":
                    Menu.DisplayBox("Find a venue", 6);
                    string cSearch = Menu.UserTextInput("Please enter a search term for the cinema: ", 6);
                    GetCinemasBySearchAsync(cSearch).Wait();
                    break;

                case "S":
                    Menu.DisplayBox("Search By Title", 6);
                    string mSearch = Menu.UserTextInput("Please enter a search term for the movie title: ", 6);
                    GetMoviesBySearchTermAsync(mSearch).Wait();
                    break;

                case "G":
                    Menu.DisplayBox("Movies by genre", 6);
                    List<string> genres = new List<string> { "Horror", "Comedy", "Fantasy", "Action", "Family", "Romance" };
                    Menu.DisplayOptions(genres);
                    int gSearch = Menu.UserInputPrompt(genres.Count, 6);
                    Console.WriteLine("\n\n");
                    GetMoviesByGenreAsync(gSearch).Wait();
                    break;

                case "X":
                    Console.WriteLine("Quitting application...");
                    break;
            }
            Console.WriteLine("\n");
            Console.ReadKey(true);
        }
    }
}

// Updated code metrics
// Maintainability Index 87 (Green), Cyclo Complexity 78, Inheritance Depth 1, Class Coupling 36
