using System;
using System.IO;
using System.Windows.Forms;
using gizindir.data;

namespace gizindir
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string sessionTokenFile = "session_token.txt";

            if (File.Exists(sessionTokenFile))
            {
                string token = File.ReadAllText(sessionTokenFile).Trim();
                if (!string.IsNullOrEmpty(token))
                {
                    var repo = new UserRepository();
                    var user = repo.GetUserBySessionToken(token);

                    if (user != null)
                    {
                        bool isCompleted = repo.IsProfileCompleted(user.Email);

                        if (!isCompleted)
                            Application.Run(new ProfileForm(user));
                        else
                            Application.Run(new Main(user.Email));

                        return;
                    }
                }
            }

            // Oturum yoksa onboarding ekranına yönlendir
            Application.Run(new Onboarding());
        }
    }
}
