using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;

namespace Language
{
    public static class languages
    {
        public class message
        {
            public string text;
            public string title;
        }

        public static class messages
        {
            public static message feedback_selected = new message();
            public static message feedback_sent = new message();
            public static message update_apply_error = new message();
            public static message crash_sent = new message();
            public static message no_internet_connection = new message();
            public static message no_update = new message();
            public static message server_down = new message();
            public static message dbupdated = new message();
            public static message browsers = new message();
            public static message delete = new message();

            public static void en()
            {
                feedback_selected.text = "Have you selected all \"useless applications\", that are installed ?";
                feedback_selected.title = "Are you sure?";
                feedback_sent.text = "Succesfuly sent to server! Thanks for your feedback!";
                feedback_sent.title = "Thank you";
                update_apply_error.text = "Error while applying update! Do you want to retry ?";
                update_apply_error.title = "Error!";
                crash_sent.text = "Thank you for sending your crash report!";
                crash_sent.title = "Thank you!";
                no_internet_connection.text = "Cannot connect to the internet! Please ensure you are connected and try again.";
                no_internet_connection.title = "No internet connection!";
                no_update.text = "You have the newest copy of UC Uninstaller.";
                no_update.title = "Up to date!";
                server_down.text = "Update server is down for maintenance. Please try it later.";
                server_down.title = "Server down!";
                dbupdated.text = "Database has been updated to the newest version!";
                dbupdated.title = "Database has been updated";
                browsers.title = "Please close your browsers";
                browsers.text = "Some uninstallers require to close your browsers. Do you want UC Uninstaller to try to close them for you ?";
                delete.title = "Invalid items found";
                delete.text = "UC Uninstaller found some invalid registry items. Do you want to delete them? Note: backup can be found in Documents.";
            }

            public static void cs()
            {
                feedback_selected.text = "Zaškrtli jste všechny \"nepotřebné aplikace\" které jsou nainstalovány ?";
                feedback_selected.title = "Jste si jisti?";
                feedback_sent.text = "Úspěšně odesláno na server! Děkuji za váš feedback!";
                feedback_sent.title = "Děkuji!";
                update_apply_error.text = "Chyba při provádění aktualizace! Chcete to zkusit znovu ?";
                update_apply_error.title = "Chyba!";
                crash_sent.text = "Děkuji za odeslání zprávy o pádu aplikace!";
                crash_sent.title = "Děkuji!";
                no_internet_connection.text = "Nemohu se připojit na internet! Ujistěte se, že jste připojeni na internet a zkuste to znovu.";
                no_internet_connection.title = "Připojení na internet nedostupné!";
                no_update.text = "Máte nejnovější verzi UC Uninstalleru.";
                no_update.title = "Aktuální!";
                server_down.text = "Aktualizační server není dostupný z důvodu údržby. Zkuste to prosím později.";
                server_down.title = "Server není dostupný!";
                dbupdated.text = "Databáze byla aktualizována na nejnovější verzi!";
                dbupdated.title = "Databáze byla aktualizována";
                browsers.title = "Prosím zavřete prohlížeče";
                browsers.text = "Některé odinstalátory vyžadují zavření všech prohlížečů. Má se je UC Uninstaller pokusit zavřít za vás ?";
                delete.title = "Nalezeny neplatné položky";
                delete.text = "UC Uninstaller nalezl neplatné položky. Chcete je odstranit? Poznámka: záloha se nachází ve složce Dokumenty.";

            }

        }



        public static class controls
        {
            public static string version { get; set; }
            public static string application { get; set; }
            public static string database { get; set; }
            public static string programmer { get; set; }
            public static string testing { get; set; }
            public static string uninstall { get; set; }
            public static string advanced { get; set; }
            public static string basic { get; set; }
            public static string send_feedback { get; set; }
            public static string autoupdate { get; set; }
            public static string cancel { get; set; }
            public static string send { get; set; }
            public static string crashed { get; set; }
            public static string describe { get; set; }
            public static string lang { get; set; }
            public static string openfolder { get; set; }
            public static string maximize { get; set; }
            public static string minimize { get; set; }
            public static string restore { get; set; }
            public static string name { get; set; }
            public static string close { get; set; }
            public static string planansky { get; set; }
            public static string musal { get; set; }
            public static string kantova { get; set; }
            public static string thanks { get; set; }
            public static string updatecheck { get; set; }
            public static string autoadvmode { get; set; }
            public static string disable_silent { get; set; }
            public static string basic_settings { get; set; }
            public static string adv_settings { get; set; }
            public static string email { get; set; }
            public static string clean { get; set; }
            public static string uninstalling { get; set; }
            public static string remember { get; set; }
            public static string autoclose { get; set; }
            public static string noautoclose { get; set; }
            public static string bautoclose { get; set; }
            public static string yes { get; set; }
            public static string no { get; set; }
            public static string rclean { get; set; }
                 
            public static void en()
            {
                version = "Version";
                application = "Application:";
                database = "Database:";
                programmer = "Programmer";
                testing = "Testing:";
                uninstall = "Uninstall now!";
                advanced = "Advanced mode";
                basic = "Basic mode";
                send_feedback = "Send feedback";
                autoupdate = "Automatically update on startup";
                cancel = "Cancel";
                send = "Send";
                crashed = "UC Uninstaller just crashed!";
                describe = "Describe what did you do when program crashed";
                lang = "Language:";
                openfolder = "Open containing folder";
                maximize = "Maximize";
                minimize = "Minimize";
                restore = "Restore Down";
                close = "Close";
                name = "Program name";
                planansky = "For testing on touchscreen devices...";
                musal = "Many special features were implemented thanks to him!";
                kantova = "For her patience in listening my programming crap...";
                thanks = "Special thanks:";
                updatecheck = "Check for updates";
                autoadvmode = "Automatically start in advanced mode";
                disable_silent = "Disable silent uninstall (use this when uninstallation fails)";
                basic_settings = "Basic";
                adv_settings = "Advanced";
                email = "E-mail (optional)";
                clean = "Your computer is clean";
                uninstalling = "Uninstalling...";
                remember = "Remember";
                autoclose = "Automatically";
                noautoclose = "Manually";
                bautoclose = "Browser closing";
                yes = "Yes";
                no = "No";
                rclean = "Clean registry";
            }

            public static void cs()
            {
                version = "Verze";
                application = "Aplikace:";
                database = "Databáze:";
                programmer = "Programátor:";
                testing = "Testování:";
                uninstall = "Odinstalovat nyní!";
                advanced = "Pokročilý mód";
                basic = "Základní mód";
                send_feedback = "Odeslat feedback";
                autoupdate = "Při startu automaticky aktualizovat";
                cancel = "Zrušit";
                send = "Odeslat";
                crashed = "UC Uninstaller právě spadl!";
                describe = "Popište co jste dělali když k tomu došlo";
                lang = "Jazyk:";
                openfolder = "Otevřít složku s programem";
                maximize = "Maximalizovat";
                minimize = "Minimalizovat";
                restore = "Obnovení z max.";
                close = "Zavřít";
                name = "Název programu";
                planansky = "Za testování na dotykových zařízeních...";
                musal = "Díky němu byla implementována spousta dodatečných funkcí!";
                kantova = "Za její trpělivost při poslouchání mých programátorských keců...";
                thanks = "Speciální poděkování:";
                updatecheck = "Zkontrolovat aktualizace";
                autoadvmode = "Automaticky spustit v pokročilém módu";
                disable_silent = "Vypnout tichou odinstalaci (použijte, pokud odinstalace selže)";
                basic_settings = "Základní";
                adv_settings = "Pokročilé";
                email = "E-mail (volitelné)";
                clean = "Váš počítač je čistý";
                uninstalling = "Odinstalovávám...";
                remember = "Zapamatovat";
                autoclose = "Automaticky";
                noautoclose = "Ručně";
                bautoclose = "Zavírání prohlížečů";
                yes = "Ano";
                no = "Ne";
                rclean = "Čistit registry";
            }
        }

        public static string getlanguage()
        {
            if(Properties.Settings.Default.language!=null|Properties.Settings.Default.language!="") return Properties.Settings.Default.language;
            else return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        }

        public static void setlanguage(string language)
        {
            if ((Properties.Settings.Default.language == null | Properties.Settings.Default.language == "" )& language == null) { Properties.Settings.Default.language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName; Properties.Settings.Default.Save(); }
            else if (language != null) { Properties.Settings.Default.language = language; Properties.Settings.Default.Save(); }
            switch (Properties.Settings.Default.language)
            {
                default:
                    {
                        messages.en();
                        controls.en();
                    }
                    break;

                case ("cs"):
                    {
                        messages.cs();
                        controls.cs();
                    }
                    break;
            }
        }



    }
}
