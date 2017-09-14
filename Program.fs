// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Text.RegularExpressions
open System.Runtime.InteropServices
open Microsoft.Extensions.Configuration

[<DllImport("user32.dll", CharSet = CharSet.Auto)>]
extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

let SPI_SETDESKWALLPAPER = 20;
let SPIF_UPDATEINIFILE = 0x01;
let SPIF_SENDWININICHANGE = 0x02;
let rand = Random()

let config = ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();

let getImages dir =
    Directory.EnumerateFiles(dir)
    |> Seq.map(fun f -> f.ToLower())
    |> Seq.filter(fun f -> Regex.Match(f, config.Item("imageRegexMatch"), RegexOptions.Compiled).Success)

let pickRandom items = 
    items
    |> Seq.map(fun f -> (rand.Next(100), f))
    |> Seq.sortBy fst
    |> Seq.take 1
    |> Seq.map snd

let setWallpaper image =
    SystemParametersInfo(
        SPI_SETDESKWALLPAPER,
        0,
        image,
        SPIF_UPDATEINIFILE ||| SPIF_SENDWININICHANGE
    ) |> ignore

[<EntryPoint>]
let main argv =
    getImages(config.Item("wallpaperDirectory"))
    |> pickRandom
    |> Seq.iter(setWallpaper)
    0 // return an integer exit code
