using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ColorMC.Core.Helpers;
using ColorMC.Core.Net;
using ColorMC.Core.Objs;
using ColorMC.Core.Utils;
using ColorMC.Gui.Manager;
using SkiaSharp;

namespace ColorMC.Gui.Utils;

/// <summary>
/// 图片处理
/// </summary>
public static class ImageUtils
{
    public static string Local { get; private set; }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="dir"></param>
    public static void Init(string dir)
    {
        Local = dir + "image/";

        Directory.CreateDirectory(Local);
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>位图</returns>
    public static async Task<Bitmap?> Load(string url, bool zoom)
    {
        if (!Directory.Exists(Local))
        {
            Directory.CreateDirectory(Local);
        }
        var sha1 = HashHelper.GenSha256(url);
        if (File.Exists(Local + sha1))
        {
            return new Bitmap(Local + sha1);
        }
        else
        {
            try
            {
                var data1 = await WebClient.GetStreamAsync(url);
                if (data1.State)
                {
                    if (zoom)
                    {
                        using var image1 = SKBitmap.Decode(data1.Stream!);
                        using var image2 = Resize(image1, 100, 100);
                        using var data = image2.Encode(SKEncodedImageFormat.Png, 100);
                        PathHelper.WriteBytes(Local + sha1, data.AsSpan().ToArray());
                        return new Bitmap(Local + sha1);
                    }
                    else
                    {
                        using var stream1 = new MemoryStream();
                        data1.Stream!.CopyTo(stream1);
                        PathHelper.WriteBytes(Local + sha1, stream1.ToArray());
                        return new Bitmap(Local + sha1);
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Logs.Error(App.Lang("ImageUtils.Error2"), e);
            }

            return ImageManager.GameIcon;
        }
    }

    /// <summary>
    /// 混合像素
    /// </summary>
    /// <param name="rgba">源</param>
    /// <param name="mix">目标</param>
    /// <returns>结果</returns>
    public static SKColor Mix(SKColor rgba, SKColor mix)
    {
        double ap = mix.Alpha / 255;
        double dp = 1 - ap;

        return new SKColor((byte)(mix.Red * ap + rgba.Red * dp),
            (byte)(mix.Green * ap + rgba.Green * dp),
            (byte)(mix.Blue * ap + rgba.Blue * dp));
    }

    /// <summary>
    /// 获得背景图
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="value">模糊度</param>
    /// <param name="lim">分辨率限制</param>
    /// <returns></returns>
    public static Task<Bitmap?> MakeBackImage(string file, int value, int lim)
    {
        return Task.Run(async () =>
        {
            try
            {
                Stream? stream1 = null;
                if (file.StartsWith("https://") || file.StartsWith("http://"))
                {
                    var res = await WebClient.DownloadClient.GetAsync(file);
                    stream1 = res.Content.ReadAsStream();
                }
                else if (file.StartsWith("ColorMC.Gui"))
                {
                    var assm = Assembly.GetExecutingAssembly();
                    stream1 = assm.GetManifestResourceStream(file)!;
                }
                else if (SystemInfo.Os == OsType.Android)
                {
                    file = ColorMCGui.RunDir + "BG";
                    stream1 = PathHelper.OpenRead(file);
                }
                else
                {
                    stream1 = PathHelper.OpenRead(file);
                }

                if (stream1 == null)
                {
                    return null;
                }
                if (value > 0 || (lim != 100 && lim > 0))
                {
                    var image = SKBitmap.Decode(stream1);
                    if (lim != 100 && lim > 0)
                    {
                        int x = (int)(image.Width * (float)lim / 100);
                        int y = (int)(image.Height * (float)lim / 100);
                        var img1 = image.Resize(new SKSizeI(x, y), SKFilterQuality.High);
                        image.Dispose();
                        image = img1;
                    }

                    if (value > 0)
                    {
                        var image1 = new SKBitmap(image.Width, image.Height);
                        var canvas = new SKCanvas(image1);

                        var paint = new SKPaint
                        {
                            ImageFilter = SKImageFilter.CreateBlur(value, value)
                        };

                        canvas.DrawBitmap(image, new SKPoint(0, 0), paint);
                        canvas.Flush();
                        image.Dispose();
                        image = image1;
                    }
                    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                    return new Bitmap(data.AsStream());
                }
                else
                {
                    return new Bitmap(stream1);
                }
            }
            catch (Exception e)
            {
                Logs.Error(App.Lang("ImageUtils.Error1"), e);
                return null;
            }
        });
    }

    /// <summary>
    /// 图片等比缩放
    /// </summary>
    /// <param name="image">图片</param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static SKBitmap Resize(SKBitmap image, int width, int height)
    {
        int newWidth;
        int newHeight;

        // 横屏图片
        if (image.Width > image.Height)
        {
            newWidth = width;
            newHeight = (int)((float)image.Height / image.Width * newWidth);
        }
        // 竖屏图片
        else
        {
            newHeight = height;
            newWidth = (int)(newHeight * ((float)image.Width / image.Height));
        }
        return image.Resize(new SKSizeI(newWidth, newHeight), SKFilterQuality.High);
    }
}
