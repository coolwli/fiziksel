
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            string json = serializer.Serialize(rows);
            string script = $"<script> data = {json}; initializeTable(); screenName = 'vmscreen'; </script>";
            ClientScript.RegisterStartupScript(this.GetType(), "initializeData", script);
