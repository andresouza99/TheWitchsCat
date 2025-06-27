using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoSenac
{
    internal static class Program
    {
        //public const string connectionString = "Server = DESKTOP-IBJFDFE\\SQLEXPRESS; integrated Security = True;";
        public static string connectionString = "";

        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //busca o nome da máquina e anota o endereço da conexão com o bd local
            connectionString = "Server = " + Environment.MachineName + "\\SQLEXPRESS; integrated Security = True";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
