using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace aes_server
{
    public partial class aes_encrypt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // this page will be initiated by GET request from Arduino

                string originalText = "mirror mirror on the wall who is the fairest of them all";

                string base64str = aes.AesEncrypt(originalText);

                // send out the encrypted string to Arduino
                Response.Write(base64str);
            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
            }
        }
    }
}