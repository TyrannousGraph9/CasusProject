class TutorialTexts
{
    public string GetText(int index)
    {
        switch (index)
        {
            case 0:
                return text1;
            case 1:
                return text2;
            case 2:
                return text3;
            case 3:
                return text4;
            case 4:
                return text5;
            default:
                return "Error: Text not found";
        }
    }

    string text1 = "Wilt u de instructies volgen?";
    string text2 = "Klik hier om een melding te maken van wat u heeft gezien.";
    string text3 = "De melding bevat een naam, foto, locatie, en informatie het dier of plant.";
    string text4 = "Klik hier om alle gemelde inheemse dieren en planten te zien.";
    string text5 = "Hier ziet u een lijst en kunt u op een plant of dier klikken voor meer informatie.";

}