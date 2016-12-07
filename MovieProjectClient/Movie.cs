using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MovieModel 
{
    public enum Certification
    {
        [Display(Name = "G")]
        IFCOG,
        [Display(Name = "PG")]
        IFCOPG,
        [Display(Name = "12A")]
        IFCO12A,
        [Display(Name = "15A")]
        IFCO15A,
        [Display(Name = "16")]
        IFCO16,
        [Display(Name = "18")]
        IFCO18
    };

    public enum Genre
    {
        Horror,
        Comedy,
        Fantasy,
        Action,        
        Family,        
        Romance
    };

    public class Movie
    {
        
        [Key, Required, MaxLength(10)]
        public String MovieID { get; set; }
        [Required]
        public String Title { get; set; }
        [Required]
        public Certification Certification { get; set; }
        [Required]
        public Genre Genre { get; set; }
        public String Description { get; set; }
        [Required, Display(Name = "Run Time")]
        // Minutes
        public double RunTime { get; set; }
        [Display(Name = "Show Time")]
        public TimeSpan ShowTime
        {
            get
            {
                var datetime = new DateTime(2010, 5, 23, 00, 00, 00);
                var time = datetime.TimeOfDay;
                if (Genre == Genre.Horror)
                {
                    datetime = new DateTime(2010, 5, 23, 22, 30, 00);
                    time = datetime.TimeOfDay;
                }
                else if (Genre == Genre.Action)
                {
                    datetime = new DateTime(2010, 5, 23, 20, 30, 00);
                    time = datetime.TimeOfDay;
                }
                else if (Genre == Genre.Family)
                {
                    datetime = new DateTime(2010, 5, 23, 14, 00, 00);
                    time = datetime.TimeOfDay;
                }
                else if (Genre == Genre.Romance)
                {
                    datetime = new DateTime(2010, 5, 23, 21, 00, 00);
                    time = datetime.TimeOfDay;
                }
                else if (Genre == Genre.Comedy)
                {
                    datetime = new DateTime(2010, 5, 23, 18, 30, 00);
                    time = datetime.TimeOfDay;
                }
                else if (Genre == Genre.Fantasy)
                {
                    datetime = new DateTime(2010, 5, 23, 16, 00, 00);
                    time = datetime.TimeOfDay;
                }
                else
                {
                    datetime = new DateTime(01, 01, 01, 00, 00, 00);
                    time = datetime.TimeOfDay;
                }
                return time;
            }

        }

        //compare times for showings
        public string MovieNow(TimeSpan ShowTime)
        {
            int compareValue = ShowTime.CompareTo(DateTime.Now.TimeOfDay);
            if (compareValue < 0)                                                      //Time is in the past
                return "Next showing is tomorrow at: " + ShowTime;
            else if (compareValue == 0)                                                //Time is now
                return "Next showing is now: " + ShowTime;
            else // compareValue > 0                                                   //Time is in the future
                return "Next showing is later today at: " + ShowTime;
        }

        [JsonIgnore]
        public virtual List<Cinema> Cinemas { get; set; }

    }


}

