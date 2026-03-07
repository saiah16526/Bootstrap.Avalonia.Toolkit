using Avalonia;
using Avalonia.Controls;

namespace Bootstrap.Avalonia.Toolkit.Behaviors;

public static class Rounded
{
    public static readonly AttachedProperty<bool> EnableProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("Enable", typeof(Rounded));

    static readonly Dictionary<string, double> V = new()
    {
        ["default"] = 5.25,
        ["sm"] = 3.5,
        ["lg"] = 7,
        ["xl"] = 14,
        ["xxl"] = 28,
        ["pill"] = 700,
        ["circle"] = 700
    };

    static Rounded()
    {
        EnableProperty.Changed.AddClassHandler<Control>((c,e)=>{
            if(e.NewValue is true){
                c.Classes.CollectionChanged+=(_,_)=>Parse(c);
                Parse(c);
            }
        });
    }

    static void Parse(Control c)
    {
        double tl=0,tr=0,br=0,bl=0; bool m=false;

        foreach(var cls in c.Classes)
        {
            if(string.IsNullOrEmpty(cls)||!cls.StartsWith("rounded")) continue;
            var p=cls.Split('-'); string pos="all",sz="default";
            if(p.Length==2) sz=p[1]; else if(p.Length==3){pos=p[1];sz=p[2];}

            if(!V.TryGetValue(sz,out var v)) continue; m=true;

            switch(pos){
                case "all": tl=tr=br=bl=v; break;
                case "top": tl=tr=v; break;
                case "bottom": bl=br=v; break;
                case "start": tl=bl=v; break;
                case "end": tr=br=v; break;
            }
        }

        if(!m) return;
        var r=new CornerRadius(tl,tr,br,bl);

        if(c is Border b) b.CornerRadius=r;
        else if(c is Button bt) bt.CornerRadius=r;
    }
}