using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace LibraryManagementSystem
{
    public abstract class LibraryItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PublicationYear { get; set; }

        protected LibraryItem(int id, string title, int publicationYear)
        {
            Id = id;
            Title = title;
            PublicationYear = publicationYear;
        }

        public abstract void DisplayInfo();

        public virtual decimal CalculateLateReturnFee(int daysLate)
        {
            return daysLate * 0.50m;
        }
    }

    public interface IBorrowable
    {
        DateTime? BorrowDate { get; set; }
        DateTime? ReturnDate { get; set; }
        bool IsAvailable { get; set; }
        void Borrow();
        void Return();
    }

    public class Book : LibraryItem, IBorrowable
    {
        public string Author { get; set; }
        public int Pages { get; set; }
        public string Genre { get; set; } = string.Empty;

        public DateTime? BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Book(int id, string title, int publicationYear, string author)
            : base(id, title, publicationYear)
        {
            Author = author;
        }

        public void Borrow()
        {
            if (!IsAvailable)
            {
                Console.WriteLine($"Book '{Title}' is currently not available.");
                return;
            }
            BorrowDate = DateTime.Now;
            IsAvailable = false;
            Console.WriteLine($"Book '{Title}' has been borrowed on {BorrowDate:yyyy-MM-dd}.");
        }

        public void Return()
        {
            ReturnDate = DateTime.Now;
            IsAvailable = true;
            Console.WriteLine($"Book '{Title}' returned on {ReturnDate:yyyy-MM-dd}.");
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Book: {Title} by {Author}, Genre: {Genre}, Pages: {Pages}, Year: {PublicationYear}, Available: {IsAvailable}");
        }

        public override decimal CalculateLateReturnFee(int daysLate)
        {
            return daysLate * 0.75m;
        }
    }

    public class DVD : LibraryItem, IBorrowable
    {
        public string Director { get; set; }
        public int Runtime { get; set; }
        public string AgeRating { get; set; } = string.Empty;

        public DateTime? BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsAvailable { get; set; } = true;

        public DVD(int id, string title, int publicationYear, string director)
            : base(id, title, publicationYear)
        {
            Director = director;
        }

        public void Borrow()
        {
            if (!IsAvailable)
            {
                Console.WriteLine($"DVD '{Title}' is currently not available.");
                return;
            }
            BorrowDate = DateTime.Now;
            IsAvailable = false;
            Console.WriteLine($"DVD '{Title}' has been borrowed on {BorrowDate:yyyy-MM-dd}.");
        }

        public void Return()
        {
            ReturnDate = DateTime.Now;
            IsAvailable = true;
            Console.WriteLine($"DVD '{Title}' returned on {ReturnDate:yyyy-MM-dd}.");
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"DVD: {Title} by {Director}, Runtime: {Runtime} minutes, Age Rating: {AgeRating}, Year: {PublicationYear}, Available: {IsAvailable}");
        }

        public override decimal CalculateLateReturnFee(int daysLate)
        {
            return daysLate * 1.00m;
        }
    }

    public class Magazine : LibraryItem
    {
        public int IssueNumber { get; set; }
        public string Publisher { get; set; } = string.Empty;

        public Magazine(int id, string title, int publicationYear, int issueNumber)
            : base(id, title, publicationYear)
        {
            IssueNumber = issueNumber;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Magazine: {Title}, Issue #{IssueNumber}, Publisher: {Publisher}, Year: {PublicationYear}");
        }
    }

    public class Library
    {
        private List<LibraryItem> items = new();

        public void AddItem(LibraryItem item)
        {
            items.Add(item);
        }

        public LibraryItem? SearchByTitle(string title)
        {
            return items.FirstOrDefault(item => item.Title.ContainsIgnoreCase(title));
        }

        public void DisplayAllItems()
        {
            foreach (var item in items)
            {
                item.DisplayInfo();
            }
        }

        public bool UpdateItemTitle(int id, ref string newTitle)
        {
            var item = items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                string oldTitle = item.Title;
                item.Title = newTitle;
                newTitle = oldTitle;
                return true;
            }
            return false;
        }

        public ref LibraryItem GetItemReference(int id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == id)
                {
                    return ref CollectionsMarshal.AsSpan(items)[i];
                }
            }
            throw new Exception("Item not found");
        }
    }

    public record BorrowRecord(int ItemId, string Title, DateTime BorrowDate, DateTime? ReturnDate, string BorrowerName)
    {
        public string LibraryLocation { get; init; }
    }

    public static class StringExtensions
    {
        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            return source?.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    public class LibraryItemCollection<T> where T : LibraryItem
    {
        private readonly List<T> items = new();

        public void Add(T item) => items.Add(item);
        public T GetItem(int index) => items[index];
        public int Count => items.Count;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var library = new Library();

            var book = new Book(1, "The Great Gatsby", 1925, "F. Scott Fitzgerald")
            {
                Genre = "Fiction",
                Pages = 180
            };

            var dvd = new DVD(2, "Inception", 2010, "Christopher Nolan")
            {
                Runtime = 148,
                AgeRating = "PG-13"
            };

            var magazine = new Magazine(3, "National Geographic", 2023, 45)
            {
                Publisher = "National Geographic Society"
            };

            library.AddItem(book);
            library.AddItem(dvd);
            library.AddItem(magazine);

            Console.WriteLine("=== All Items ===");
            library.DisplayAllItems();

            Console.WriteLine("\n=== Borrowing ===");
            book.Borrow();
            book.Return();

            Console.WriteLine("\n=== Search ===");
            var found = library.SearchByTitle("gatsby");
            if (found != null) found.DisplayInfo();
            else Console.WriteLine("Item not found.");

            Console.WriteLine("\n=== Late Fee Calculation ===");
            Console.WriteLine($"Book Late Fee (3 days): {book.CalculateLateReturnFee(3):C}");
            Console.WriteLine($"DVD Late Fee (3 days): {dvd.CalculateLateReturnFee(3):C}");

            Console.WriteLine("\n=== Ref Parameter Test ===");
            string newTitle = "Gatsby Modified";
            if (library.UpdateItemTitle(1, ref newTitle))
            {
                Console.WriteLine($"Updated title successfully. Old title was: {newTitle}");
            }

            Console.WriteLine("\n=== Ref Return Test ===");
            ref var itemRef = ref library.GetItemReference(2);
            Console.WriteLine($"Original Title: {itemRef.Title}");
            itemRef.Title += " (Updated via Ref)";
            Console.WriteLine($"New Title: {itemRef.Title}");

            Console.WriteLine("\n=== Borrow Record History ===");
            List<BorrowRecord> history = new();
            var record = new BorrowRecord(book.Id, book.Title, book.BorrowDate ?? DateTime.Now, book.ReturnDate, "John Doe")
            {
                LibraryLocation = "Main Branch"
            };
            history.Add(record);
            foreach (var r in history)
            {
                Console.WriteLine(r);
            }
        }
    }
}