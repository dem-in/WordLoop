using WordLoop.Core.Decks;

namespace WordLoop.Core.Tests.Decks;

public class SampleCardsTests
{
    [Fact]
    public void HasTwentyCards()
    {
        Assert.Equal(20, SampleCards.All.Count);
    }

    [Fact]
    public void CardIds_AreUniqueAndMatchHeadwordAndPos()
    {
        Assert.Equal(SampleCards.All.Count, SampleCards.All.Select(c => c.Id).Distinct().Count());
        Assert.All(SampleCards.All, c => Assert.Equal($"{c.Headword}#{c.Pos}", c.Id));
    }

    [Fact]
    public void RequiredFields_AreFilled()
    {
        Assert.All(SampleCards.All, c =>
        {
            Assert.False(string.IsNullOrWhiteSpace(c.Headword));
            Assert.False(string.IsNullOrWhiteSpace(c.Pos));
            Assert.False(string.IsNullOrWhiteSpace(c.Cefr));
            Assert.InRange(c.Translations.Count, 1, 3);
            Assert.All(c.Translations, t => Assert.False(string.IsNullOrWhiteSpace(t)));
            Assert.False(string.IsNullOrWhiteSpace(c.Example.En));
            Assert.False(string.IsNullOrWhiteSpace(c.Example.Ru));
        });
    }

    [Fact]
    public void CoversOptionalFieldVariants()
    {
        // The screen must be checkable both with and without the optional fields.
        Assert.Contains(SampleCards.All, c => c.Pattern is not null);
        Assert.Contains(SampleCards.All, c => c.Pattern is null);
        Assert.Contains(SampleCards.All, c => c.Ipa is not null);
        Assert.Contains(SampleCards.All, c => c.Ipa is null);
    }
}
