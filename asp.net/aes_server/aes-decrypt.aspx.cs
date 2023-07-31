using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace aes_server
{
    public partial class aes_decrypt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // obtaining the GET request, the data appended after the symbol "?"
                // which is also called Query String
                string base64str = Request.Url.Query;

                // remove the 1st character "?"
                base64str = base64str.Remove(0, 1);

                // decode the URL characters of "%2B", "%2F", "%3D" into "+", "/", "="
                base64str = Server.UrlDecode(base64str);

                string text = aes.AesDecrypt(base64str);

                // send out the decrypted text to Arduino
                Response.Write(text);
            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
            }
        }
    }
}