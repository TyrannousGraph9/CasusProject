class TutorialTextsAdmin
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
            default:
                return "Error: Text not found";
        }
    }

    string text1 = "Wilt u de instructies volgen?";
    string text2 = "Deze lijst bevat alle meldingen die nog in behandeling staan, klik op een melding om deze te keuren of af te keuren";
    string text3 = "Klik hier om een melding dat nog in behandeling staat goed te keuren";
    string text4 = "Klik hier om een melding dat nog in behandeling staat af te keuren";

}