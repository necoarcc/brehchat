using System.Reflection;

namespace brehchat_dotnet
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            if (Config.InSettings)
            {
                Close();
                return;
            }
            Config.InSettings = true;
            var assembly = Assembly.GetExecutingAssembly();
            verlabel.Text = "v";
            verlabel.Text += assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? assembly.GetName().Version?.ToString() ?? "?.?.? (Can't get version)";
#if DEBUG
            verlabel.Text += "(!!DEBUG!!)";
            verlabel.ForeColor = Color.Red;
#endif
        }

        private void AboutForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Config.InSettings = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
