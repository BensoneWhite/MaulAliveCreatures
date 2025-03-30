using Menu.Remix.MixedUI;
using UnityEngine;

namespace MaulAliveCreatures;

public class OptionsMenu : OptionInterface
{
    public static Configurable<bool> DisableSounds;

    public static Configurable<bool> NoMeatExtra;

    public static Configurable<bool> CantMaulCreatures;

    public static Configurable<bool> CanMaul;

    public static Configurable<bool> CanMaulPups;

    public static Configurable<bool> EnableExtraStun;

    public static Configurable<bool> EnableMaulAllies;

    public static Configurable<bool> NoMaulMSC;

    public static Configurable<float> MaulDamageExtra;

    private OpFloatSlider MaulDamageExtraSlider;

    private OpLabel MaulDamageExtraCheckBox;

    public OptionsMenu()
    {
        DisableSounds = base.config.Bind<bool>("DisableSounds", false, (ConfigurableInfo)null);
        NoMeatExtra = base.config.Bind<bool>("NoMeatExtra", false, (ConfigurableInfo)null);
        CantMaulCreatures = base.config.Bind<bool>("CantMaulCreatures", false, (ConfigurableInfo)null);
        CanMaul = base.config.Bind<bool>("CanMaul", false, (ConfigurableInfo)null);
        CanMaulPups = base.config.Bind<bool>("CanMaulPups", false, (ConfigurableInfo)null);
        EnableExtraStun = base.config.Bind<bool>("EnableExtraStun", false, (ConfigurableInfo)null);
        EnableMaulAllies = base.config.Bind<bool>("EnableMaulAllies", false, (ConfigurableInfo)null);
        NoMaulMSC = base.config.Bind<bool>("EnableMSCMaul", false, (ConfigurableInfo)null);
        MaulDamageExtra = base.config.Bind<float>("MaulDamageExtra", 1f, (ConfigAcceptableBase)(object)new ConfigAcceptableRange<float>(1f, 10000f));
    }

    public override void Update()
    {
        ((OptionInterface)this).Update();
        MaulDamageExtraSlider.colorFill = Color.red;
    }

    public override void Initialize()
    {
        var opTab1 = new OpTab((OptionInterface)(object)this, "Options");
        base.Tabs = (OpTab[])(object)new OpTab[1] { opTab1 };
        var tab1Container = new OpContainer(new Vector2(0f, 0f));
        opTab1.AddItems((UIelement[])(object)new UIelement[1] { (UIelement)tab1Container });
        UIelement[] obj =
        [
            (UIelement)new OpCheckBox(DisableSounds, 10f, 540f)
            {
                description = OptionInterface.Translate("Disables the custom sounds from the mod")
            },
            (UIelement)new OpLabel(45f, 540f, "No sounds", false),
            (UIelement)new OpCheckBox(NoMeatExtra, 10f, 500f)
            {
                description = OptionInterface.Translate("Disables the extra meat Feature from mauling alive creatures")
            },
            (UIelement)new OpLabel(45f, 500f, "No Extra Meat", false),
            (UIelement)new OpCheckBox(CantMaulCreatures, 10f, 460f)
            {
                description = OptionInterface.Translate("Don't let everyone can maul")
            },
            (UIelement)new OpLabel(45f, 460f, "No Maul", false)
        ];
        var val = new OpFloatSlider(MaulDamageExtra, new Vector2(5f, 245f), 200, (byte)2, true)
        {
            max = 10000f,
            min = 1f,
            hideLabel = false
        };
        OpFloatSlider val2 = val;
        MaulDamageExtraSlider = val;
        obj[6] = (UIelement)val2;
        var val3 = new OpLabel(45f, 420f, "Maul Damage", false);
        OpLabel val4 = val3;
        MaulDamageExtraCheckBox = val3;
        obj[7] = (UIelement)val4;
        obj[8] = (UIelement)new OpCheckBox(CanMaul, 270f, 540f)
        {
            description = OptionInterface.Translate("Enables maul for every slugcat")
        };
        obj[9] = (UIelement)new OpLabel(300f, 540f, "Everyone mauls", false);
        obj[10] = (UIelement)new OpCheckBox(CanMaulPups, 270f, 500f)
        {
            description = OptionInterface.Translate("The wawas ARE SO YUMMERS")
        };
        obj[11] = (UIelement)new OpLabel(300f, 500f, "Maul Pups", false);
        obj[12] = (UIelement)new OpCheckBox(EnableExtraStun, 270f, 460f)
        {
            description = OptionInterface.Translate("Grab with extra stun")
        };
        obj[13] = (UIelement)new OpLabel(300f, 460f, "Easier Grab", false);
        obj[14] = (UIelement)new OpCheckBox(EnableMaulAllies, 270f, 420f)
        {
            description = OptionInterface.Translate("Maul your friends in coop")
        };
        obj[15] = (UIelement)new OpLabel(300f, 420f, "Maul Allies", false);
        obj[16] = (UIelement)new OpCheckBox(NoMaulMSC, 270f, 380f)
        {
            description = OptionInterface.Translate("Disables Saint and spear master to maul creatures")
        };
        obj[17] = (UIelement)new OpLabel(300f, 380f, "No MSC maul", false);
        UIelement[] UIArrayElements2 = (UIelement[])(object)obj;
        opTab1.AddItems(UIArrayElements2);
    }
}