# itemcounter
Track items in games using speech recognition and speech synthesis. Windows only.



# POC plan

1st iteration: quick and dirty
- [x] Recognize common item names: mega, red, yellow, blue1/2
- [x] When item name is recognized, e scheduler to add necessary reminders (assuming C# has scheduler)
- [x] When the game starts, say "start", to synchronize internal clock and the game clock
- [x] When time comes, invoke speech synthesis to say how much time left
- [x] As soon as item is recognized, say it's respawn seconds
- [x] Blame Press0K for everything :)

