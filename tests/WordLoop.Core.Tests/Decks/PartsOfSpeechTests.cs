using WordLoop.Core.Decks;

namespace WordLoop.Core.Tests.Decks;

public class PartsOfSpeechTests
{
    [Fact]
    public void All_ContainsTheCodesFromSpec()
    {
        string[] expected = ["n", "v", "adj", "adv", "prep", "pron", "det", "conj", "num", "exclam", "modal", "aux", "phr"];

        Assert.Equal(expected.Order(), PartsOfSpeech.All.Order());
    }

    [Fact]
    public void Constants_AreAllKnown()
    {
        string[] constants =
        [
            PartsOfSpeech.Noun, PartsOfSpeech.Verb, PartsOfSpeech.Adjective, PartsOfSpeech.Adverb,
            PartsOfSpeech.Preposition, PartsOfSpeech.Pronoun, PartsOfSpeech.Determiner, PartsOfSpeech.Conjunction,
            PartsOfSpeech.Number, PartsOfSpeech.Exclamation, PartsOfSpeech.Modal, PartsOfSpeech.Auxiliary,
            PartsOfSpeech.PhrasalVerb,
        ];

        Assert.Equal(PartsOfSpeech.All.Order(), constants.Order());
    }

    [Fact]
    public void EveryCode_HasNonEmptyLabel()
    {
        Assert.All(PartsOfSpeech.All, code => Assert.False(string.IsNullOrWhiteSpace(PartsOfSpeech.Label(code))));
    }

    [Theory]
    [InlineData("n")]
    [InlineData("phr")]
    public void IsKnown_KnownCode_True(string code)
    {
        Assert.True(PartsOfSpeech.IsKnown(code));
    }

    [Theory]
    [InlineData("xyz")]
    [InlineData("")]
    [InlineData("N")]
    [InlineData(" n")]
    public void IsKnown_UnknownCode_False(string code)
    {
        Assert.False(PartsOfSpeech.IsKnown(code));
    }

    [Fact]
    public void Label_UnknownCode_Throws()
    {
        var error = Assert.Throws<ArgumentException>(() => PartsOfSpeech.Label("xyz"));

        Assert.Contains("xyz", error.Message);
    }

    [Fact]
    public void SampleCards_UseOnlyKnownCodes()
    {
        Assert.All(SampleCards.All, card => Assert.True(PartsOfSpeech.IsKnown(card.Pos), card.Id));
    }
}
