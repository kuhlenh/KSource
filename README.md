# Demo Instructions
You can see the whole talk on [Ch9](https://channel9.msdn.com/Events/Build/2018/BRK2145) or [YouTube](https://www.youtube.com/watch?v=6ozHCNVmrcw).

## Setup
1. Install Visual Studio 2017 15.8
1. Download source code for demo
1. Have a breakpoint on the open brace of ```GetArrivalTimesForRouteName(...)``` (MyStopInfo, line 150)
1. Tools>Options>Text Editor>C#>Advanced>Enable navigation to decompiled assemblies
1. [Visual Studio Live Share](https://marketplace.visualstudio.com/items?itemName=MS-vsliveshare.vsls-vs)
1. [Visual Studio IntelliCode](https://marketplace.visualstudio.com/items?itemName=VisualStudioExptTeam.VSIntelliCode)

## Demo Steps
### Phase I
1. Open solution and show how quickly Test Explorer discovers unit tests

    > Note: **responsive icons** which show pending, running, and completed test runs
    * Show off the new **hierarchy view** to organize tests by project, namespace, and class

2. Right-click and debug failing test, ```TestGetArrivals()```
3. Hit the breakpoint in ```GetArrivalTimesForRouteName(...)``` (MyStopInfo, line 150).

    * **F10** and **F11** to step over and into
    * Use **Run To Click** to run debugger execution to mouse
    * Step until ```var busTimes = arrivalData
                            .Where(a=> a.PredictedArrivalTime != null && (Int64)a.PredictedArrivalTime > 0)
                            .Select(a => BusHelpers.ConvertMillisecondsToUTC(a.PredictedArrivalTime));```. Put a breakpoint on the ```BusHelpers``` expression with **F9**.

        > Note: this is external source

    * Press **F5** to continue debugging until it hits that breakpoint. Use **Navigation to decompiled assemblies** to see the definition of ```BusHelpers.ConvertMillisecondsToUTC(...)```. We worked with ILSpy team to build in a decompiler to see method bodies, not just signatures.
    * Even better to see the *real source*. Use F11 to Step Into ```BusHelpers.ConvertMillisecondsToUTC``` and SourceLink will download your source on the fly while debugging. Step through the whole method, inspect variables, etc.
        > Note: ```TestGetArrivals()``` isn't failing because of external source.
4.  Step inside the ```foreach``` until you get to after the ```delta``` calculation. Here we can see ```delta.TotalMinutes``` is a very long decimal.
    * Use **Edit and Continue** to select ```delta.TotalMinutes```, introduce a local variable with **Ctrl+.**, and use **Math.Round(..., 0)** to round the result. Stepping through the next value in the ```foreach``` will show that this correctly rounded the total minutes.
5. Go back to Test Explorer and re-run your tests. See that ```TestGetArrivals()``` is now passing.

      > Note: tests are organized alphabetically rather than pass/fail. This is so you can track your changes more easily.

### Phase II
1. We still have a failing test, ```TestGetArrivalsNull()```. Double-click the test in Test Explorer to navigate to the test. By using **CodeLens**, we can see that Allison touched this test last.
1. Call Allison and start a **Live Share** session by clicking the Share button in the top-right of VS. Send the link copied to your clipboard to Allison.
1. Turn on **Live Unit Testing** via Test > Live Unit Testing > Start.
    * Live  Unit Testing removes all the manual steps we just did in Phase I with unit testing by automatically figuring out which tests are impacting by our code changes, running only those tests, and updating icons in the editor so we don't lose the context of our development.
    * Red X = at least one failing test hits this line; Green Check = all passing tests hit this line; Blue Dash = no unit tests hit this line
    * Click an icon to see what tests hit the line, hover to see Exception info, and double-click to navigate to test.
1. The host of the LiveShare session starts debugging the test  ```TestGetArrivalsNull()```. The guest of LiveShare can now step over and then into ```GeocodeHelpers.ValidateLatLon(lat, lon)```. From here, they will hit an Exception and see the **Exception Helper**.
    * We know that ```lat``` is null from our Helper. Go to the parameter list and press **Ctrl+.** and add a null check.
    * On the host's machine, we can see that Live Unit Testing has updated live and let us know our tests are now passing.
1. End the Live Share session.

### Phase III
1. Running this Alexa skill at home, I realized it wasn't giving me bus times for the cloeset bus stop to me. Use **Go To All** with **Ctrl+T** to quickly navigate to files/types/symbols/members. Type ```getloc``` and navigate to method ```GetRouteAndStopForLocation```.
1. This line lets us know that no calculations were done ```Stop minDistStop = stops.First();```, we just grab the first bus stop returned.
1. Use **Ctrl+K,C** to comment out that line.
1. Write some new code to find the closest bus stop:
```csharp
 var min = (from stop in stops
            select GeocodeHelpers.CalculateDistanceFormula(lat, stop.Lat, lon, stop.Lon)).Min();
 Stop minDistStop = stops.Where(x => GeocodeHelpers.CalculateDistanceFormula(lat, x.Lat, lon, x.Lon) == min).FirstOrDefault();
```
1. Use **Ctrl+.** to generate the missing method ```GeocodeHelpers.CalculateDistanceFormula```. Use **Ctrl+Click** to create a hyperlink on the ```GeocodeHelpers.CalculateDistanceFormula``` symbol to Go To Definition.
    * Change the return type of the method to ```double```
    * Use **Ctrl+.** to Change Method Signature and reorder parameters so that the location pairs are organized by type
    * Inside the method body, type ```var distance = Math.```. See the IntelliCode suggestions.
    * Now change ```var``` to ```double``` and use **Ctrl+Space** to bring up completion again and show a different list based on the new context

### Phase IV
1. Live Unit Testing hasn't changed despite me changing my code...meaning I don't have a unit test that properly tests this scenario.
1. From the editor, select the ```CalculateDistanceFormula``` and use **Ctrl+E,E** to send the code snippet to the **C# Interactive Window**
    * Execute the following statements:
```csharp
(string lat, string lon) conventionCenter = ("47.611959", "-122.332893");
(double lat, double lon) NewStop = (47.61196, -122.33489);
(double lat, double lon) MyStop = (47.613937, -122.33416);
```

  * Execute ```CalculateDistanceFormula(conventionCenter.lat, conventionCenter.lon, MyStop.lat, MyStop.lon)```. Press **Ctrl+Enter** to force excecution when your cursor is not at the end of the line.
  * Use **Alt+Up** to access Interactive history and change MyStop to NewStop and commit again.

  > Note that the new bus stop you're going to add to the json file is indeed a closer bus stop

1. Use **Ctrl+T** and type "json" to navigate to file StopsForLoc.json
1. Add the following to the list of bus stops:
```
{
        "code": '"700"',
        "direction": '"NW"',
        "id": '"Testing-1-2-3"',
        "lat": 47.611960,
        "locationType": 0,
        "lon": -122.332893,
        "name": '"NewStop"',
        "routeIds": [ "1_100264", "40_100236" ],
        "wheelchairBoarding": '"UNKNOWN"'
}
```
  * Notice that the json is invalid because of escaped characters. Use **multi-caret mode** commands to fix.
  * Select ```"\``` and press **Shift+Alt+Ins** to add new selections of escaped characters and **Delete** (make sure **Fn** is not on!)
  * Now select ```\"``` and  use **Ctrl+Ins** to select all instances of escaped characters and delete.
1. Go back to file MyStopInfo.cs and see that Live Unit Testing is reporting failures. Investigating a red X and hovering over a failing tests shows the "Testing1-2-3" that we just added, so our method is correctly returning the closest stop.
1. Delete that new stop...to make tests pass again.
1. Use Team Explorer status bar to see git status. Commit recent changes.

### Phase V
1. Go to GeocodeHelpers.cs and look at the dotted suggestions in the file
1. Show that you can use **Ctrl+.** to apply a code suggestion with a lightbulb, or you can apply the opposite as a refactoring (e.g., flip between implicit and explicit types)
1. Right-click in Solution Explorer and Add > New Item. Type "editorconfig"
  * We now have starter templates in here
  * Use VS IntelliCode to generate an .editorconfig based on your current coding conventions in your codebase
  > Note: You no longer need to close and reopen all files to see changes from an edited .editorconfig
1. Open the **Error List** and filter to just "Current Document"
1. Go to Tools>Options>Text Editor>C#>Code Style>Formatting to see new Format Document cleanup configuraiton
1. Change settings and click 'OK'
1. **Format Document** with *Ctrl+K,D** and see all the errors in Error List disappear!

### Phase VI
1. Everyone wants more refactorings, so here is a tour de source
1. Go to Error List and show Warnings in whole solution. See "unreachable code". Navigate and use **Ctrl+.** to remove unreachable code.
1. Convert if to switch
1. Add missing switch cases
1. Convert to interpolated string
1. Add missing accessibility modifiers
1. Make readonly
1. Order modifiers
1. Convert LINQ to foreach, foreach to for
1. Invert if
