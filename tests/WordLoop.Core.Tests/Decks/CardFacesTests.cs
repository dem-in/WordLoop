using WordLoop.Core.Decks;

namespace WordLoop.Core.Tests.Decks;

public class CardFacesTests
{
    private static Card MakeCard(
        string pos = "v",
        string? pattern = null,
        string? ipa = "/əˈtʃiːv/",
        params string[] translations) =>
        new(
            Id: "achieve#" + pos,
            Headword: "achieve",
            Pos: pos,
            Pattern: pattern,
            Ipa: ipa,
            Cefr: "A2",
            Unit: null,
            Translations: translations.Length > 0 ? translations : ["достигать", "добиваться"],
            Example: new CardExample(
                "She achieved her goal of running a marathon.",
                "Она достигла своей цели — пробежала марафон."));

    [Fact]
    public void Front_ShowsHeadwordAndPartOfSpeechLabel()
    {
        var front = CardFaces.Front(MakeCard());

        Assert.Equal("achieve", front.Headword);
        Assert.Equal("гл.", front.PartOfSpeech);
    }

    [Fact]
    public void Front_WithPattern_ShowsPattern()
    {
        var front = CardFaces.Front(MakeCard(pos: "phr", pattern: "put sth off"));

        Assert.Equal("put sth off", front.Pattern);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Front_MissingPattern_IsNull(string? pattern)
    {
        var front = CardFaces.Front(MakeCard(pattern: pattern));

        Assert.Null(front.Pattern);
    }

    [Theory]
    [InlineData("n", "сущ.")]
    [InlineData("v", "гл.")]
    [InlineData("adj", "прил.")]
    [InlineData("adv", "нареч.")]
    [InlineData("prep", "предл.")]
    [InlineData("pron", "мест.")]
    [InlineData("det", "опред.")]
    [InlineData("conj", "союз")]
    [InlineData("num", "числ.")]
    [InlineData("exclam", "межд.")]
    [InlineData("modal", "мод. гл.")]
    [InlineData("aux", "вспом. гл.")]
    [InlineData("phr", "фраз. гл.")]
    public void Front_KnownPartOfSpeech_HasRussianAbbreviation(string pos, string expected)
    {
        Assert.Equal(expected, CardFaces.Front(MakeCard(pos: pos)).PartOfSpeech);
    }

    [Fact]
    public void Front_UnknownPartOfSpeech_FallsBackToCode()
    {
        Assert.Equal("xyz", CardFaces.Front(MakeCard(pos: "xyz")).PartOfSpeech);
    }

    [Fact]
    public void Back_JoinsTranslationsWithCommaInOriginalOrder()
    {
        var back = CardFaces.Back(MakeCard(translations: ["достигать", "добиваться", "достичь"]));

        Assert.Equal("достигать, добиваться, достичь", back.Translations);
    }

    [Fact]
    public void Back_SingleTranslation_HasNoComma()
    {
        Assert.Equal("достигать", CardFaces.Back(MakeCard(translations: ["достигать"])).Translations);
    }

    [Fact]
    public void Back_ShowsIpaAndExample()
    {
        var back = CardFaces.Back(MakeCard());

        Assert.Equal("/əˈtʃiːv/", back.Ipa);
        Assert.Equal("She achieved her goal of running a marathon.", back.ExampleEn);
        Assert.Equal("Она достигла своей цели — пробежала марафон.", back.ExampleRu);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Back_MissingIpa_IsNull(string? ipa)
    {
        Assert.Null(CardFaces.Back(MakeCard(ipa: ipa)).Ipa);
    }
}
