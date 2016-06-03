using System.ComponentModel;
using System.Drawing;

using Gdk;

public static class MyExtensions
{
    public static Bitmap ToBitmap(this Pixbuf pix)
    {
        TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
        return (Bitmap)tc.ConvertFrom(pix.SaveToBuffer("png")); 
    }

    public static int GetWidth(this Gdk.Window gdkWindow)
    {
        int w, h;
        gdkWindow.GetSize(out w, out h);
        return w;
    }

    public static int GetHeight(this Gdk.Window gdkWindow)
    {
        int w, h;
        gdkWindow.GetSize(out w, out h);
        return h;
    }
}