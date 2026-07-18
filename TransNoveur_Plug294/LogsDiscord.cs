using System.Text;
using System.Net.Http;
using Life.Network;
using UnityEngine;
using Newtonsoft.Json;

namespace TransNoveur_Plug294
{
    // Logs Discord du plugin (embeds envoyés au webhook)
    public partial class TransNoveurPlug294
    {
        // File d'attente pour respecter la limite du webhook (~30 msg/min)
        private static readonly Queue<string> logQueue = new Queue<string>();
        private static bool logWorkerRunning = false;
        private static readonly HttpClient logHttp = new HttpClient();

        // Couleur du cadre selon l'événement
        private const int LogVert = 5763719;     // achats
        private const int LogBleu = 3447003;     // garage
        private const int LogJaune = 15844367;   // banque
        private const int LogOrange = 15105570;  // administration
        private const int LogRouge = 15548997;   // erreurs

        private static string Who(Player player)
        {
            return player.FullName + " (" + player.steamId + ")";
        }

        private static string Coords(Vector3 position)
        {
            return "X " + position.x.ToString("0.0") + " | Y " + position.y.ToString("0.0") + " | Z " + position.z.ToString("0.0");
        }

        // fields = paires nom/valeur de l'embed
        public void Log(string title, int color, params string[] fields)
        {
            if (string.IsNullOrEmpty(config.discordWebhook))
                return;
            var fieldList = new List<object>();
            for (int i = 0; i + 1 < fields.Length; i += 2)
                fieldList.Add(new { name = fields[i], value = fields[i + 1], inline = false });
            string payload = JsonConvert.SerializeObject(new
            {
                embeds = new object[]
                {
                    new
                    {
                        title = title,
                        color = color,
                        fields = fieldList,
                        footer = new { text = "TransNoveur_Plug294 • " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") }
                    }
                }
            });
            lock (logQueue)
            {
                logQueue.Enqueue(payload);
                if (logWorkerRunning)
                    return;
                logWorkerRunning = true;
            }
            _ = SendLogs();
        }

        // L'envoi ne doit jamais gêner le jeu : erreurs avalées
        private async Task SendLogs()
        {
            while (true)
            {
                string payload;
                lock (logQueue)
                {
                    if (logQueue.Count == 0)
                    {
                        logWorkerRunning = false;
                        return;
                    }
                    payload = logQueue.Dequeue();
                }
                try
                {
                    await logHttp.PostAsync(config.discordWebhook, new StringContent(payload, Encoding.UTF8, "application/json"));
                }
                catch (Exception e)
                {
                    Console.WriteLine("[TransNoveur_Plug294] Envoi log Discord échoué : " + e.Message);
                }
                await Task.Delay(2500);
            }
        }

        // Erreur du plugin : console serveur + embed rouge
        public void LogError(string context, Exception e)
        {
            Console.WriteLine("[TransNoveur_Plug294] ERREUR " + context + " : " + e);
            string details = e.GetType().Name + " : " + e.Message;
            if (details.Length > 1000)
                details = details.Substring(0, 1000);
            Log("❌ Erreur Plugin", LogRouge, "📍 Contexte", context, "📝 Détails", details);
        }
    }
}
