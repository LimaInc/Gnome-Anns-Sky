public class StaticBSC : IBacterialStateComponent
{
    public void Update(float delta, ExoWorld w, BacterialState bs)
    {
        bs.TryGetBacteria(BacteriumType.OXYGEN, out Bacteria bOxy);
        bs.TryGetBacteria(BacteriumType.NITROGEN, out Bacteria bNito);
        bs.TryGetBacteria(BacteriumType.CARBON_DIOXIDE, out Bacteria bCO2);
        bOxy.Amount = 5;
        bNito.Amount = 2;
        bCO2.Amount = 1;
    }
}