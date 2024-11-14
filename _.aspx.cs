using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web.UI.WebControls;

public partial class ManageUsers : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadUsers();
        }
    }

    private Configuration GetConfig()
    {
        // F dizinindeki dosya yolu
        string path = @"F:\path\to\your\web.config";
        var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = path };
        return ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
    }

    private void LoadUsers()
    {
        var users = new List<string>();
        var config = GetConfig();
        var authorizationSection = (AuthorizationSection)config.GetSection("system.web/authorization");

        foreach (AuthorizationRule rule in authorizationSection.Rules)
        {
            if (rule.Action == AuthorizationRuleAction.Allow)
            {
                foreach (string user in rule.Users)
                {
                    users.Add(user);
                }
            }
        }

        UsersGridView.DataSource = users.ConvertAll(user => new { Username = user });
        UsersGridView.DataBind();
    }

    protected void AddUserButton_Click(object sender, EventArgs e)
    {
        string newUser = NewUserTextBox.Text.Trim();
        if (!string.IsNullOrEmpty(newUser))
        {
            var config = GetConfig();
            var authorizationSection = (AuthorizationSection)config.GetSection("system.web/authorization");

            AuthorizationRule newRule = new AuthorizationRule(AuthorizationRuleAction.Allow);
            newRule.Users.Add(newUser);
            authorizationSection.Rules.Add(newRule);

            config.Save(ConfigurationSaveMode.Modified);
            MessageLabel.Text = $"{newUser} eklendi.";
            LoadUsers();
        }
        else
        {
            MessageLabel.Text = "Kullanıcı adı boş olamaz.";
        }
    }

    protected void UsersGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Remove")
        {
            string userToRemove = e.CommandArgument.ToString();
            var config = GetConfig();
            var authorizationSection = (AuthorizationSection)config.GetSection("system.web/authorization");

            foreach (AuthorizationRule rule in authorizationSection.Rules)
            {
                if (rule.Action == AuthorizationRuleAction.Allow && rule.Users.Contains(userToRemove))
                {
                    rule.Users.Remove(userToRemove);
                    break;
                }
            }

            config.Save(ConfigurationSaveMode.Modified);
            MessageLabel.Text = $"{userToRemove} kaldırıldı.";
            LoadUsers();
        }
    }
}
