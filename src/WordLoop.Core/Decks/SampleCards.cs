namespace WordLoop.Core.Decks;

/// <summary>
/// Temporary hard-coded cards for slice 2 (spec 12), until decks are loaded from files (slice 4).
/// Covers cards with and without IPA/pattern so the screen can be checked for both.
/// </summary>
public static class SampleCards
{
    public static IReadOnlyList<Card> All { get; } =
    [
        Word("achieve", "v", "/əˈtʃiːv/", "A2", ["достигать", "добиваться"],
            "She achieved her goal of running a marathon.", "Она достигла своей цели — пробежала марафон."),
        Word("bank", "n", "/bæŋk/", "A1", ["банк", "берег"],
            "I need to go to the bank before it closes.", "Мне нужно зайти в банк, пока он не закрылся."),
        Word("beautiful", "adj", "/ˈbjuːtɪfl/", "A1", ["красивый", "прекрасный"],
            "What a beautiful view!", "Какой красивый вид!"),
        Word("quickly", "adv", "/ˈkwɪkli/", "A1", ["быстро"],
            "He quickly finished his breakfast.", "Он быстро доел завтрак."),
        Word("between", "prep", "/bɪˈtwiːn/", "A1", ["между"],
            "The shop is between the bank and the post office.", "Магазин находится между банком и почтой."),
        Word("decide", "v", "/dɪˈsaɪd/", "A1", ["решать", "принимать решение"],
            "We decided to stay at home.", "Мы решили остаться дома."),
        Word("although", "conj", "/ɔːlˈðəʊ/", "A2", ["хотя"],
            "Although it was raining, we went for a walk.", "Хотя шёл дождь, мы пошли гулять."),
        Word("environment", "n", "/ɪnˈvaɪrənmənt/", "A2", ["окружающая среда", "обстановка"],
            "We must protect the environment.", "Мы должны защищать окружающую среду."),
        Word("however", "adv", "/haʊˈevə(r)/", "A2", ["однако", "тем не менее"],
            "The plan was good. However, it didn't work.", "План был хорош. Однако он не сработал."),
        Word("knowledge", "n", "/ˈnɒlɪdʒ/", "B1", ["знание", "знания"],
            "She has a good knowledge of history.", "У неё хорошие знания истории."),
        Word("recognize", "v", "/ˈrekəɡnaɪz/", "B1", ["узнавать", "признавать"],
            "I didn't recognize you with short hair.", "Я не узнал тебя с короткой стрижкой."),
        Word("reliable", "adj", "/rɪˈlaɪəbl/", "B1", ["надёжный"],
            "This car is very reliable.", "Эта машина очень надёжная."),
        Word("suggest", "v", "/səˈdʒest/", "A2", ["предлагать", "советовать"],
            "I suggest we take a taxi.", "Предлагаю взять такси."),
        Word("whose", "pron", "/huːz/", "A2", ["чей"],
            "Whose bag is this?", "Чья это сумка?"),
        Word("hello", "exclam", "/həˈləʊ/", "A1", ["привет", "здравствуйте"],
            "Hello, how are you?", "Привет, как дела?"),
        Word("thousand", "num", "/ˈθaʊznd/", "A1", ["тысяча"],
            "The ticket costs a thousand roubles.", "Билет стоит тысячу рублей."),
        Word("must", "modal", "/mʌst/", "A1", ["должен", "обязан"],
            "You must wear a seat belt.", "Ты должен пристегнуться."),
        Phrasal("put off", "put sth off", "B1", ["откладывать", "переносить"],
            "Don't put off your homework until tomorrow.", "Не откладывай домашнее задание на завтра."),
        Phrasal("give up", "give sth up", "A2", ["бросать", "отказываться от"],
            "He gave up smoking last year.", "В прошлом году он бросил курить."),
        Phrasal("look after", "look after sb", "A2", ["заботиться о", "присматривать за"],
            "Can you look after my cat this weekend?", "Можешь присмотреть за моей кошкой в эти выходные?"),
    ];

    private static Card Word(string headword, string pos, string ipa, string cefr, string[] translations, string en, string ru) =>
        new($"{headword}#{pos}", headword, pos, Pattern: null, ipa, cefr, Unit: null, translations, new CardExample(en, ru));

    private static Card Phrasal(string headword, string pattern, string cefr, string[] translations, string en, string ru) =>
        new($"{headword}#phr", headword, "phr", pattern, Ipa: null, cefr, Unit: null, translations, new CardExample(en, ru));
}
