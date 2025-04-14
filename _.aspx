<!DOCTYPE html>
<html lang="tr">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Administrator Users</title>
    <style>
      * {
        margin: 0;
        padding: 0;
        box-sizing: border-box;
      }

      body {
        font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
        background-color: #f9fafb;
        color: #1f2937;
      }

      header.main-header {
        background-color: #111827; /* siyah ton */
        padding: 20px;
        text-align: center;
        font-size: 24px;
        font-weight: 600;
        color: #f3f4f6; /* açık gri yazı */
        box-shadow: 0 1px 4px rgba(0, 0, 0, 0.05);
        border-bottom: 1px solid #1f2937;
      }

      .layout {
        display: flex;
        min-height: 100vh;
      }

      .sidebar {
        width: 220px;
        background-color: #1f2937; /* koyu sidebar */
        padding: 20px 0;
        border-right: 1px solid #374151;
        box-shadow: 2px 0 5px rgba(0, 0, 0, 0.05);
      }

      .sidebar a {
        display: block;
        color: #d1d5db;
        padding: 12px 24px;
        text-decoration: none;
        border-radius: 8px;
        transition: background-color 0.3s ease, color 0.3s ease;
        font-weight: 500;
      }

      .sidebar a:hover {
        background-color: #374151;
        color: #ffffff;
      }

      .sidebar a.active {
        background-color: #64748b;
        color: #ffffff;
      }

      .content-area {
        flex: 1;
        padding: 40px;
      }
    </style>
  </head>
  <body>
    <header class="main-header">Administrator Users</header>

    <div class="layout">
      <nav class="sidebar">
        <a class="active">Administrators</a>
        <a href="remote.aspx">Remote Desktop Users</a>
        <a href="local.aspx">Local Users</a>
        <a href="windowsnetwork.aspx">Windows Networks</a>
      </nav>

      <main class="content-area"></main>
    </div>
  </body>
</html>
