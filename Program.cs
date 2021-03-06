﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading.Tasks;

using static System.Console;

var counter = new ItemCounter();

counter.Add("mega", 35, 10, 5);
counter.Add("red", 25, 7, 4);
counter.Add("yellow", 25, 7, 4);
counter.Add("blue one", 25, 7, 4);
counter.Add("blue two", 25, 7, 4);
counter.Add("blue three", 25, 7, 4);
counter.Add("smoke", 60, 7, 4);
counter.Add("slow", 60, 7, 4);

var synthesizer = new SpeechSynthesizer { 
    Rate = 4
};
synthesizer.SetOutputToDefaultAudioDevice();

counter.ItemEvent += (_, e) =>
{
    WriteLine($"{e.Item1} {e.Item2}");
    synthesizer.SpeakAsync(e.Item2);
};

var recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));

var grammar = new Grammar(new GrammarBuilder(new Choices(counter.GetCommands())));

recognizer.LoadGrammar(grammar);
recognizer.SpeechRecognized += counter.SpeechRecognized;
recognizer.SetInputToDefaultAudioDevice();
recognizer.RecognizeAsync(RecognizeMode.Multiple);

ReadKey();


class ItemCounter
{
    private readonly Dictionary<string, (int, int[])> reminders = new();

    public void Add(string name, int respawn, params int[] notification) => reminders[name] = (respawn, notification);

    public string[] GetCommands() => reminders.Keys.Append("start").ToArray();

    DateTime zeroTime = DateTime.Now;

    public event EventHandler<(TimeSpan, string)> ItemEvent;

    public void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    {
        WriteLine($"{DateTime.Now}. Recognized text: {e.Result.Text}");

        if (e.Result.Text == "start")
        {
            zeroTime = DateTime.Now;
            WriteLine();
            return;
        }

        var (resp, rem) = reminders[e.Result.Text];
        var nextRespawn = (DateTime.Now - zeroTime).Add(TimeSpan.FromSeconds(resp));

        Task.Delay(TimeSpan.FromSeconds(resp)).ContinueWith(p => ItemEvent(this, (DateTime.Now - zeroTime, $"{e.Result.Text} now")));

        

        foreach (var i in rem)
        {
            var delay = resp - i;
            Task.Delay(TimeSpan.FromSeconds(delay)).ContinueWith(p => ItemEvent(this,
                (DateTime.Now - zeroTime, $"{e.Result.Text} in {i}, {nextRespawn.Seconds} ")));
        }

        ItemEvent(this,
                (DateTime.Now - zeroTime, $"{e.Result.Text} is at {nextRespawn.Seconds}"));
    }
}
