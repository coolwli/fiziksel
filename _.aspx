<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>servsitory List</title>
    <style>
      body {
        background-color: #f0f0f0;
        color: #333;
        font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
        margin: 0;
        padding: 0;
      }

      .header {
        background-color: #343436;
        color: white;
        padding: 8px;
        font-size: 24px;
        font-weight: bold;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      }

      .header a {
        color: white;
        text-decoration: none;
      }

      #page-name {
        color: #5e5ce6;
      }

      .container {
        max-width: 1200px;
        margin: 40px auto;
        padding: 20px;
        display: flex;
        gap: 20px;
        flex-wrap: wrap;
      }

      .serv-group {
        flex: 1;
        min-width: 300px;
        padding: 20px;
        border-radius: 8px;
        background-color: #fff;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      }

      .serv-group-title {
        font-size: 24px;
        font-weight: bold;
        margin-bottom: 15px;
        color: #196fd1;
        text-align: center;
      }

      .serv-item {
        padding: 15px 0;
        border-bottom: 1px solid #ddd;
      }

      .serv-item:last-child {
        border-bottom: none;
      }

      .serv-title {
        font-size: 20px;
        font-weight: bold;
        margin: 0;
      }

      .serv-meta {
        font-size: 14px;
        color: #666;
      }
    </style>
  </head>

  <body>
    <div class="header">
      <a href="/">cloudmosaic</a>
      <span id="page-name">servsitory</span>
    </div>

    <div class="container">
      <div class="serv-group">
        <div class="serv-group-title">Frontend Projectsğ</div>
        <div class="serv-item">
          <h2 class="serv-title">
            UI Framework <span class="serv-meta">(Public)</span>
          </h2>
        </div>
        <div class="serv-item">
          <h2 class="serv-title">
            Responsive hsl(240, 1.9%, 20.8%)
            <span class="serv-meta">(Public)</span>
          </h2>
        </div>
      </div>
      <div class="serv-group">
        <div class="serv-group-title">Backend Projects</div>
        <div class="serv-item">
          <h2 class="serv-title">
            API Server <span class="serv-meta">(Public)</span>
          </h2>
        </div>
        <div class="serv-item">
          <h2 class="serv-title">
            Database Manager <span class="serv-meta">(Public)</span>
          </h2>
        </div>
      </div>
      <div class="serv-group">
        <div class="serv-group-title">Configuration & Utilities</div>
        <div class="serv-item">
          <h2 class="serv-title">
            System Configs <span class="serv-meta">(Public)</span>
          </h2>
        </div>
        <div class="serv-item">
          <h2 class="serv-title">
            Automation Scripts <span class="serv-meta">(Public)</span>
          </h2>
        </div>
      </div>
    </div>
    <script>
      const colorPalette = [
        "#EDBB00",
        "#0071EB",
        "#ff3037",
        "#32d158",
        "#48C9B0",
        "#0071e3",
        "#3498DB",
        "#9B59B6",
        "#E74C3C",
        "#2ECC71",
        "#32d158",
        "#63e6e2",
        "#40c8e0",
        "#64d2ff",
        "#009aff",
        "#5e5ce6",
        "#bf5af2",
      ];
    </script>
  </body>
</html>
