
public interface ICreatedByIdGameObjectVisitor
{
    void Visit(SimpleObject simpleObject);
    void Visit(TutorialOnly simpleObject);
    void Visit(SellBoard sellBoard);
    void Visit(UpgradeBoard upgradeBoard);
    void Visit(ResourceSource resourceSource);
    void Visit(ResourceStorage resourceStorage);
    void Visit(Converter converter);
    void Visit(Workbench workbench);
    void Visit(Workshop workshop);
    void Visit(Chunk chunk);
    void Visit(Dungeon dungeon);
    void Visit(Portal portal);
    void Visit(FirstPrize firstPrize);
    void Visit(FinalPrize finalPrize);
    void Visit(BootsAdsObject bootsAdsObject);
    void Visit(AdsResourceBox adsResourceBox);
}