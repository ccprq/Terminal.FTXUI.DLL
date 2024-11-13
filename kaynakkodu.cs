using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fsoc{
    internal class Program{
        public static int _buf = 3;
        public static string _sd = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        static string _input(){
            string input = "";
            int cursor = 0;
            while (true){
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key != ConsoleKey.Tab){
                    if (keyInfo.KeyChar == '\r'){
                        Console.WriteLine();
                        return input;
                    }
                    else if (keyInfo.KeyChar == '\b'){
                        if (input.Length > 0 && cursor > 0){
                            input = input.Remove(cursor - 1, 1);
                            cursor--;
                            Console.CursorVisible = false;
                            Console.SetCursorPosition(_buf, Console.CursorTop);
                            Console.Write(new string(' ', Console.WindowWidth - _buf));
                            Console.SetCursorPosition(_buf, Console.CursorTop);
                            Console.Write(input); 
                            Console.SetCursorPosition(cursor + _buf, Console.CursorTop);
                            Console.CursorVisible = true;
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.LeftArrow){
                        if (cursor > 0){
                            Console.CursorLeft--;
                            cursor--;
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.RightArrow){
                        if (cursor < input.Length){
                            Console.CursorLeft++;
                            cursor++;
                        }
                    }
                    else{
                        input = input.Insert(cursor, keyInfo.KeyChar.ToString());
                        cursor++;
                        Console.CursorVisible = false;
                        Console.SetCursorPosition(_buf, Console.CursorTop);
                        Console.Write(new string(' ', Console.WindowWidth - _buf));
                        Console.SetCursorPosition(_buf, Console.CursorTop);
                        Console.Write(input);
                        Console.SetCursorPosition(cursor + _buf, Console.CursorTop);
                        Console.CursorVisible = true;
                    }
                }
            }
        }
        public static List<string> sds = new List<string>();
        public static List<string> sds_date = new List<string>();
        public static bool _isnumber(string input){
            input = input.TrimStart('-');
            for (int i = 0; i < input.Length; i++){
                if (!char.IsDigit(input[i])){
                    return false;
                }
            }
            return true;
        }
        public static bool _file(string input){
            return File.Exists($"{_sd}\\{input}");
        }
        public static Dictionary<string, string> coms = new Dictionary<string, string>(){
            {"SD","<SD> VEYA <SD [YOL VEYA INDEX]> | DIZIN GECMISINI GOSTERIR VEYA DIZIN DEGISTIRMEK ICIN KULLANILIR"},
            {"ER","<ER> VEYA <ER [INDEX]> | BELIRTILEN INDEKSDEKI HATAYI VEYA TUM HATALARI GOSTERIR"},
            {"FILES","<FILES> | BULUNAN DIZINDEKI DOSYALARI GOSTERIR"},
            {"DIRS","<DIRS> | BULUNULAN DIZINDEKI DIZINLERI GOSTERIR"},
            {"ECHO","<ECHO [ICERIK]> | GIRILEN ICERIGI EKRANA YAZDIRIR"},
            {"DH","<DH [INDEX]> | BULUNAN DIZINI INDEKSE GORE GECMIS DIZINE AYARLAR"},
            {"DES","<DES> | BULUNULAN DIZINI MASAUSTU OLARAK AYARLAR"},
            {"HOME","<HOME> | BULUNULAN DIZINI KULLANICI DIZINI OLARAK AYARLAR"},
            {"HELP","<HELP> | BU YARDIM EKRANINI GORUNTULER"},
            {"CLEAR","<CLEAR> | EKRANI TEMIZLER, ISLEM SURESI KONSOL BUFFERINA BAGLIDIR"},
            {"DEL","<DEL [YOL VEYA INDEX]> | BELIRTILEN INDEKSDEKI VEYA YOLDAKI DIZINI, DOSYAYI SILER"},
            {"TITLE","<TITLE [ICERIK]> | KONSOL BASLIGINI DEGISTIRIR"},
            {"READ","<READ [YOL VEYA INDEX]> | BELIRTILEN DOSYAYI EKRANA YAZDIRIR"},
            {"RED","<RED> | SON KULLANILAN DIZINLERI GOSTERIR"},
            {"REF","<REF> | SON KULLANILAN DOSYALARI GOSTERIR"},
            {"MKF","<MKF [ISIM]> | BELIRTILEN ISIMDE DOSYA OLUSTURUR"},
            {"MKD","<MKD [ISIM]> | BELIRTILEN ISIMDE DIZIN OLUSTURUR"}
        };
        public static List<string> er = new List<string>();
        static void Main(string[] args){
            Console.ForegroundColor = ConsoleColor.White;
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.SetWindowSize(120, 28);
            Console.SetBufferSize(5000, 5000);
            Console.Title = "fsoc";
            Console.CursorSize = 100;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(@"
  █████▒ ██████  ▒█████   ▄████▄  
▓██   ▒▒██    ▒ ▒██▒  ██▒▒██▀ ▀█  
▒████ ░░ ▓██▄   ▒██░  ██▒▒▓█    ▄ 
░▓█▒  ░  ▒   ██▒▒██   ██░▒▓▓▄ ▄██▒
░▒█░   ▒██████▒▒░ ████▓▒░▒ ▓███▀ ░
 ▒ ░   ▒ ▒▓▒ ▒ ░░ ▒░▒░▒░ ░ ░▒ ▒  ░
 ░     ░ ░▒  ░ ░  ░ ▒ ▒░   ░  ▒   
 ░ ░   ░  ░  ░  ░ ░ ░ ▒  ░        
             ░      ░ ░  ░ ░      
                         ░ ");
            sds.Add(_sd);
            sds_date.Add($" {DateTime.UtcNow}");
            while (true){
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("fsoc");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"[{_sd}] -> 🍕");
                Console.Write(" > ");
                string com = _input();
                if (com.Trim().Contains(' ')) {
                    com = com.Trim();
                    string[] parts = com.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string path_able = com.Substring(com.IndexOf(' ') + 1);
                    if (parts[0].ToLower() == "sd") {
                        if (Directory.Exists(path_able)){
                            _sd = parts[1];
                            sds.Add(_sd);
                            sds_date.Add($" Erişim zamanı: {DateTime.UtcNow}");
                        }
                        else if (Directory.Exists($"{_sd}\\{path_able}")) {
                            _sd += $"\\{path_able}";
                            sds.Add(_sd);
                            sds_date.Add($" Erişim zamanı: {DateTime.UtcNow}");
                        }
                        else if (_isnumber(parts[1])) {
                            try {
                                _sd = Directory.GetDirectories(_sd)[int.Parse(parts[1])];
                                sds.Add(_sd);
                                sds_date.Add($" {DateTime.UtcNow}");
                            }
                            catch (Exception e) {
                                Console.WriteLine("Belirtilen sayı dizin sayısından daha fazla.");
                                er.Add(e.ToString() + "\n" + e.Message + $" Alınan hata zamanı: {DateTime.UtcNow}");
                            }
                        }
                        else {
                            Console.WriteLine($"Belirtilen dizin bulunamadı, dizin:{parts[1]}");
                        }
                    }
                    else if (parts[0].ToLower() == "echo") {
                        Console.WriteLine(path_able);
                    }
                    else if (parts[0].ToLower() == "dh") {
                        if (_isnumber(parts[1])) {
                            try {
                                _sd = sds[int.Parse(parts[1])];
                            }
                            catch (Exception e) {
                                Console.WriteLine("[HATA]Belirtilen sayı geçmiş dizin sayısından daha fazla.");
                                er.Add(e.ToString() + "\n" + e.Message + $" Alınan hata zamanı: {DateTime.UtcNow}");
                            }
                        }
                        else {
                            Console.WriteLine("Geçmiş dizinlerde gezinmek için sadece sayı kullanın.");
                        }
                    }
                    else if (parts[0].ToLower() == "del"){
                        if (_file(path_able)){
                            File.Delete($"{_sd}\\{path_able}");
                        }
                        else if (File.Exists(path_able)){
                            File.Delete(path_able);
                        }
                        else if (Directory.Exists($"{_sd}\\{path_able}")){
                            Directory.Delete($"{_sd}\\{path_able}",true);
                        }
                        else if (Directory.Exists(path_able)){
                            Directory.Delete(path_able,true);
                        }
                        else if (_isnumber(parts[1])){
                            try{
                                if (File.Exists(Directory.GetFiles(_sd)[int.Parse(parts[1])])){
                                    File.Delete(Directory.GetFiles(_sd)[int.Parse(parts[1])]);
                                }
                                else if (Directory.Exists(Directory.GetDirectories(_sd)[int.Parse(parts[1])])){
                                    Directory.Delete(Directory.GetDirectories(_sd)[int.Parse(parts[1])]);
                                }
                                else{
                                    Console.WriteLine($"Girdiğiniz yol bulunamadı, yol:{parts[1]}");
                                }
                            }
                            catch (Exception e){
                                Console.WriteLine("[HATA]Belirtilen sayı geçmiş dizin sayısından daha fazla.");
                                er.Add(e.ToString() + "\n" + e.Message + $" Alınan hata zamanı: {DateTime.UtcNow}");
                            }
                        }
                        else{
                            Console.WriteLine($"Girdiğiniz yol bulunamadı, yol:{path_able}");
                        }
                    }
                    else if (parts[0].ToLower() == "mkd"){
                        Directory.CreateDirectory($"{_sd}\\{path_able}");
                    }
                    else if (parts[0].ToLower() == "mkf"){
                        using (File.Create($"{_sd}\\{path_able}")){}
                    }
                    else if (parts[0].ToLower() == "read"){
                        string[] content = {};
                        if(File.Exists(path_able) || File.Exists($"{_sd}\\{path_able}")){
                            if (File.Exists(path_able)){
                                content = File.ReadAllLines(path_able);
                            }
                            else if (File.Exists($"{_sd}\\{path_able}")){
                                content = File.ReadAllLines($"{_sd}\\{path_able}");
                            }    
                        }
                        else if (_isnumber(parts[1])){
                            content = File.ReadAllLines(Directory.GetFiles(_sd)[int.Parse(parts[1])]);
                        }
                        else Console.WriteLine($"Girilen yol bulunamadı, yol:{path_able}");
                        for (int i = 0; i < content.Length; i++){
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.Write($" {i} |");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(content[i] + new string(' ', Console.WindowWidth - content[i].Length));
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else if (parts[0].ToLower() == "title"){
                        Console.Title = path_able;
                    }
                    else if (parts[0].ToLower() == "er"){
                        if (_isnumber(parts[1])){
                            try{
                                if(er.Count > 0){
                                    Console.WriteLine(er.ElementAt(int.Parse(parts[1])));
                                }
                                else{
                                    Console.WriteLine("Henüz hata yaşanmadı.");
                                }
                            }
                            catch (Exception e){
                                Console.WriteLine("[HATA]Belirtilen sayı hata geçmişindeki hata sayısından daha fazla.");
                                er.Add(e.ToString() + "\n" + e.Message + $" Alınan hata zamanı: {DateTime.UtcNow}");
                            }
                        }
                        else{
                            Console.WriteLine("Hata geçmişinden hata raporu almak için sadece sayı kullanın.");
                        }
                    }
                    else{
                        if (!string.IsNullOrWhiteSpace(com))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Girdiğiniz komut bulunamadı.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                }
                else{
                    if(com.Trim().ToLower() == "sd"){
                        Console.WriteLine("Dizin geçmişi:");
                        for(int i = 0; i < sds.Count; i++){
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write($"{i}|");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"{sds[i]} | Erişim tarihi:{sds_date[i]}");
                        }
                    }
                    else if(com.Trim().ToLower() == "red"){
                        string[] recend = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.Recent));
                        Console.WriteLine("En son kullanılan dizinler:");
                        for (int i = 0; i < recend.Length; i++){
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.Write($" {i} |");
                            Console.ForegroundColor = ConsoleColor.White;
                            try{
                                Console.WriteLine(recend[i] + new string(' ', Console.WindowWidth - recend[i].Length));
                            }
                            catch (Exception e){
                                Console.WriteLine($"{recend[i]} - [HATA]hata raporlarını kontrol edin.");
                                er.Add(e.ToString() + "\n" + e.Message + $" Alınan hata zamanı: {DateTime.UtcNow}");
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else if (com.Trim().ToLower() == "ref"){
                        string[] recend = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Recent));
                        Console.WriteLine("En son kullanılan dosyalar:");
                        for (int i = 0; i < recend.Length; i++){
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.Write($" {i} |");
                            Console.ForegroundColor = ConsoleColor.White;
                            try{
                                if(Console.WindowWidth - recend[i].Length > 0){
                                    Console.WriteLine(recend[i] + new string(' ', Console.WindowWidth - recend[i].Length));
                                }
                                else{
                                    Console.WriteLine(recend[i]);
                                }
                            }
                            catch (Exception e){
                                Console.WriteLine($"{recend[i]} - [HATA]hata raporlarını kontrol edin.");
                                er.Add(e.ToString() + "\n" + e.Message + $" Alınan hata zamanı: {DateTime.UtcNow}");
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else if(com.Trim().ToLower() == "help"){
                        Console.ForegroundColor= ConsoleColor.DarkGray;
                        Console.WriteLine($"KOMUTLAR{new string(' ',12)}KULLANIMLAR");
                        Console.ForegroundColor = ConsoleColor.White;
                        foreach (var item in coms){
                            Console.WriteLine($"{item.Key}{new string(' ',20 - item.Key.Length)}{item.Value}");
                        }
                    }
                    else if(com.Trim().ToLower() == "des"){
                        _sd = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        sds.Add(_sd);
                        sds_date.Add($" Erişim zamanı: {DateTime.UtcNow}");
                    }
                    else if(com.Trim().ToLower() == "home"){
                        _sd = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        sds.Add(_sd);
                        sds_date.Add($" Erişim zamanı: {DateTime.UtcNow}");
                    }
                    else if(com.Trim().ToLower() == "files"){
                        string[] files = Directory.GetFiles(_sd);
                        Console.WriteLine($"{_sd} dizinin dosyaları:");
                        for(int i = 0; i < files.Length; i++){
                            FileInfo file = new FileInfo(files[i]);
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write($"{i}|");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"[{file.Length} byte]{file.Name}");
                        }
                    }
                    else if(com.Trim().ToLower() == "dirs"){
                        string[] dirs = Directory.GetDirectories(_sd);
                        Console.WriteLine($"{_sd} dizinin dizinleri:");
                        for (int i = 0; i < dirs.Length; i++){
                            DirectoryInfo file = new DirectoryInfo(dirs[i]);
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write($"{i}|");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"{file.Name}");
                        }
                    }
                    else if(com.Trim().ToLower() == "er"){
                        if (er.Count > 0){
                            int i = 0;
                            Console.WriteLine("Hata geçmişi:");
                            foreach (var item in er){
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write($"{i}|");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine($"{item}");
                                i++;
                            }
                        }
                        else{
                            Console.WriteLine("Henüz hata yaşanmadı.");
                        }
                    }
                    else if(com.Trim().ToLower() == "clear"){
                        Console.Clear();
                    }
                    else{
                        if (!string.IsNullOrWhiteSpace(com)){
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Girdiğiniz komut bulunamadı.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                }
            }
        }
    }
}
