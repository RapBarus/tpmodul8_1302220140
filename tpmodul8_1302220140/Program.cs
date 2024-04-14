using System;
using System.IO;
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        double suhuBadan;
        int hariDemam;

        CovidData defaultConf = new CovidData();

        Console.Write($"Berapa suhu badan Anda saat ini? Dalam nilai {defaultConf.CovidConf.satuan_suhu}: ");
        if (!double.TryParse(Console.ReadLine(), out suhuBadan))
        {
            Console.WriteLine("Input tidak valid. Harap masukkan nilai numerik untuk suhu badan.");
            return;
        }

        Console.Write("Berapa hari yang lalu (perkiraan) anda terakhir memiliki gejala demam? ");
        if (!int.TryParse(Console.ReadLine(), out hariDemam))
        {
            Console.WriteLine("Input tidak valid. Harap masukkan nilai numerik untuk jumlah hari demam.");
            return;
        }

        bool tepatWaktu = hariDemam < defaultConf.CovidConf.batas_hari_demam;
        bool terimaFahrenheit = (defaultConf.CovidConf.satuan_suhu == "Fahrenheit") && (suhuBadan >= 97.7 && suhuBadan <= 99.5);
        bool terimaCelcius = (defaultConf.CovidConf.satuan_suhu == "Celcius") && (suhuBadan >= 36.5 && suhuBadan <= 37.5);


        if (tepatWaktu && (terimaCelcius || terimaFahrenheit))
        {
            Console.WriteLine(defaultConf.CovidConf.pesan_diterima);
            CovidConfig config = new CovidConfig("Celcius", 14, "Anda tidak diperbolehkan masuk ke dalam gedung ini", "Anda dipersilahkan untuk masuk ke dalam gedung ini");

            Console.WriteLine($"Satuan suhu: {config.satuan_suhu}");

            config.UbahSatuan();

            Console.WriteLine($"Satuan suhu setelah perubahan: {config.satuan_suhu}");



            if (config.satuan_suhu == "Celcius")
            {
                Console.Write($"Masukkan suhu badan Anda dalam derajat Celcius: ");
                while (!double.TryParse(Console.ReadLine(), out suhuBadan) || suhuBadan >= 36.5 && suhuBadan <= 37.5)
                {
                    Console.WriteLine("Input tidak valid.");
                    Console.Write($"Masukkan suhu badan Anda dalam derajat Celcius: ");
                }
            }
            else if (config.satuan_suhu == "Fahrenheit")
            {
                Console.Write($"Masukkan suhu badan Anda dalam derajat Fahrenheit: ");
                while (!double.TryParse(Console.ReadLine(), out suhuBadan) || suhuBadan >= 97.7 && suhuBadan <= 99.5)
                {
                    Console.WriteLine("Input tidak valid.");
                    Console.Write($"Masukkan suhu badan Anda dalam derajat Fahrenheit: ");
                }
            }

            Console.Write("Berapa hari yang lalu (perkiraan) Anda terakhir memiliki gejala demam? ");
            while (!int.TryParse(Console.ReadLine(), out hariDemam))
            {
                Console.WriteLine("Input tidak valid.");
                Console.Write("Berapa hari yang lalu (perkiraan) Anda terakhir memiliki gejala demam? ");
            }



            if (tepatWaktu && (terimaCelcius || terimaFahrenheit))
            {
                Console.WriteLine(defaultConf.CovidConf.pesan_diterima);
            }
            else
            {
                Console.WriteLine(defaultConf.CovidConf.pesan_ditolak);

            }
        }
        else
        {
            Console.WriteLine(defaultConf.CovidConf.pesan_ditolak);

        }
        
    }
}

public class CovidConfig
{
    public string satuan_suhu { get; set; }
    public int batas_hari_demam { get; set; }
    public string pesan_ditolak { get; set; }
    public string pesan_diterima { get; set; }

    public CovidConfig() { }

    public CovidConfig(string satuan_suhu, int batas_hari_demam, string pesan_ditolak, string pesan_diterima)
    {
        this.satuan_suhu = satuan_suhu;
        this.batas_hari_demam = batas_hari_demam;
        this.pesan_ditolak = pesan_ditolak;
        this.pesan_diterima = pesan_diterima;
    }

    public void UbahSatuan()
    {
        if (satuan_suhu == "Celcius")
        {
            satuan_suhu = "Fahrenheit";
        }
        else if (satuan_suhu == "Fahrenheit")
        {
            satuan_suhu = "Celcius";
        }
        else
        {
            throw new InvalidOperationException("Satuan suhu tidak valid.");
        }
    }
}


public class CovidData
{
    public CovidConfig CovidConf { get; private set; }
    private const string filePath = @"E:\TELKOM UNIVERSITY\TUGAS KULIAH\KONSTRUKSI PERANGKAT LUNAK (KPL)\TP_MOD_08_1302220140_Raphael Permana Barus\tpmodul8_1302220140\\covid_config.json";

    public CovidData()
    {
        try
        {
            ReadConfig();
        }
        catch (FileNotFoundException)
        {
            WriteNewConfig();
        }
        catch (JsonException)
        {
            Console.WriteLine("File konfigurasi tidak valid. Membuat konfigurasi default.");
            WriteNewConfig();
        }
    }

    private void ReadConfig()
    {
        string jsonData = File.ReadAllText(filePath);
        CovidConf = JsonSerializer.Deserialize<CovidConfig>(jsonData);
    }



    private void WriteNewConfig()
    {
        JsonSerializerOptions opts = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        string jsonString = JsonSerializer.Serialize(CovidConf, opts);
        File.WriteAllText(filePath, jsonString);
    }

    

}
