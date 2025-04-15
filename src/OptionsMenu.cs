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

    //private OpFloatSlider MaulDamageExtraSlider;

    //private OpLabel MaulDamageExtraCheckBox;

    public OptionsMenu()
    {
        DisableSounds = config.Bind("DisableSounds", false);
        NoMeatExtra = config.Bind("NoMeatExtra", false);
        CantMaulCreatures = config.Bind("CantMaulCreatures", false);
        CanMaul = config.Bind("CanMaul", false);
        CanMaulPups = config.Bind("CanMaulPups", false);
        EnableExtraStun = config.Bind("EnableExtraStun", false);
        EnableMaulAllies = config.Bind("EnableMaulAllies", false);
        NoMaulMSC = config.Bind("EnableMSCMaul", false);
        MaulDamageExtra = config.Bind("MaulDamageExtra", 1f, new ConfigAcceptableRange<float>(1f, 10000f));
    }

    public override void Update()
    {
        base.Update();
        //MaulDamageExtraSlider.colorFill = Color.red;
    }

    public override void Initialize()
    {
        var opTab1 = new OpTab(this, "Options");

        Tabs = ([opTab1]);

        var tab1Container = new OpContainer(new Vector2(0f, 0f));

        opTab1.AddItems([tab1Container]);

        UIelement[] arrayElementsTab =
        [
            new OpCheckBox(DisableSounds, 10f, 540f) { description = Translate("Disables the custom sounds from the mod") },
            new OpLabel(45f, 540f, "No sounds", false),

            new OpCheckBox(NoMeatExtra, 10f, 500f) { description = Translate("Disables the extra meat feature from mauling alive creatures") },
            new OpLabel(45f, 500f, "No Extra Meat", false),

            new OpCheckBox(CantMaulCreatures, 10f, 460f) { description = Translate("Don't let everyone can maul") },
            new OpLabel(45f, 460f, "No Maul", false),

            new OpFloatSlider(MaulDamageExtra, new Vector2(5f, 245f), 200, 2, true) { max = 10000f, min = 1f, hideLabel = false },
            new OpLabel(45f, 420f, "Maul Damage", false),

            new OpCheckBox(CanMaul, 270f, 540f) { description = Translate("Enables maul for every slugcat") },
            new OpLabel(300f, 540f, "Everyone mauls", false),

            new OpCheckBox(CanMaulPups, 270f, 500f) { description = Translate("The wawas ARE SO YUMMERS") },
            new OpLabel(300f, 500f, "Maul Pups", false),

            new OpCheckBox(EnableExtraStun, 270f, 460f) { description = Translate("Grab with extra stun") },
            new OpLabel(300f, 460f, "Easier Grab", false),

            new OpCheckBox(EnableMaulAllies, 270f, 420f) { description = Translate("Maul your friends in coop") },
            new OpLabel(300f, 420f, "Maul Allies", false),

            new OpCheckBox(NoMaulMSC, 270f, 380f) { description = Translate("Disables Saint and spear master to maul creatures") },
            new OpLabel(300f, 380f, "No MSC maul", false),
        ];

        opTab1.AddItems(arrayElementsTab);
    }
}