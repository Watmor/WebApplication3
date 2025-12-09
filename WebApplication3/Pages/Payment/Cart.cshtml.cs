using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace WebApplication3.Pages.Payment
{
    public class CartModel : PageModel
    {
        //public void OnGet()
        //{
        //    if (HttpContext.Session.GetInt32("UserId") == null)
        //    {
        //        Response.Redirect("/Users/Signin");
        //    }
        //}

        //public List<CartItemView> CartItems { get; set; } = new();
        //public decimal Subtotal { get; set; }


        //public IActionResult OnGet()
        //{
        //    int? userId = HttpContext.Session.GetInt32("UserId");
        //    if (userId == null)
        //    {
        //        return RedirectToPage("/Users/Signin");
        //    }


        //    string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=users;Integrated Security=True;Trust Server Certificate=True";


        //    using var conn = new SqlConnection(connectionString);
        //    conn.Open();


        //    string sql = @"SELECT c.CartItemID, c.Quantity,b.BookID, b.Title, b.Price, b.ImageUrl
        //                    FROM Cart c
        //                    JOIN Books b ON c.BookID = b.BookID
        //                    WHERE c.UserID = @UserID";

        //    using var cmd = new SqlCommand(sql, conn);
        //    cmd.Parameters.AddWithValue("@UserID", userId);


        //    using var reader = cmd.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        var item = new CartItemView
        //        {
        //            CartItemID = (int)reader["CartItemID"],
        //            BookID = (int)reader["BookID"],
        //            Title = reader["Title"].ToString()!,
        //            Price = (decimal)reader["Price"],
        //            ImageUrl = reader["ImageUrl"].ToString()!,
        //            Quantity = (int)reader["Quantity"]
        //        };


        //        item.Total = item.Price * item.Quantity;
        //        Subtotal += item.Total;
        //        CartItems.Add(item);
        //    }


        //    return Page();
        //}


        //public class CartItemView
        //{
        //    public int CartItemID { get; set; }
        //    public int BookID { get; set; }
        //    public string Title { get; set; }
        //    public string ImageUrl { get; set; }
        //    public decimal Price { get; set; }
        //    public int Quantity { get; set; }
        //    public decimal Total { get; set; }
        //}


        public List<CartItemView> CartItems { get; set; } = new();

        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; } = 30000; // phí c? ??nh
        public decimal Total { get; set; }

        public class CartItemView
        {
            public int CartItemID { get; set; }
            public string Title { get; set; }
            public string ImageUrl { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }

            public decimal LineTotal => Price * Quantity;
        }

        public void OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                Response.Redirect("/Users/Signin");
                return;
            }

            string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=users;Integrated Security=True;Trust Server Certificate=True";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string sql = @"
                SELECT c.CartItemID, b.Title, b.Price, b.ImageUrl, c.Quantity
                FROM Cart c
                JOIN Books b ON b.BookID = c.BookID
                WHERE c.UserID = @UserID";

                using var cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@UserID", userId.Value);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CartItems.Add(new CartItemView
                    {
                        CartItemID = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        ImageUrl = reader.IsDBNull(3) ? "/images/default.png" : reader.GetString(3),
                        Quantity = reader.GetInt32(4)
                    });
                }
            }

            // Tính subtotal
            Subtotal = CartItems.Sum(i => i.LineTotal);

            // T?ng c?ng
            Total = Subtotal + Shipping;
        }
    }
}
