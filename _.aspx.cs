            TokenManager tokenManager = new TokenManager(_httpClient);
            _token = await tokenManager.GetTokenAsync(vcenter.InnerText);
            if (_token == null) {
                Response.Write("Şu anda Performance & Disk verilerini görüntüleyemiyoruz lütfen daha sonra deneyiniz"+_token);
                return;
            }
