# NormalRace v4

An open-world racing game I'm building solo as a personal project in Unity/C#. The real goal behind it isn't just shipping a game — it's forcing myself to actually understand car physics: how torque gets distributed to the wheels, how steering should change with speed, and how braking and traction affect the way a car *feels* to drive, instead of just following a tutorial blindly.

Play Store link:
https://play.google.com/store/apps/details?id=com.AhGame.NormalRacev4&pcampaignid=web_share

---

## Why this project exists

I didn't start this to pump out a polished commercial title. I started it to get hands-on experience building a semi-realistic driving system from scratch on top of Unity's `WheelCollider`. Every system in the game exists because I tried something, felt it was wrong, and went back to fix it.

If you open `CarController.cs`, you'll notice several commented-out blocks that are basically old versions of the same logic. That's not laziness — it's a deliberate history of past attempts. I keep them around so I can compare, or roll back if a "better" idea turns out not to actually be better once it's in the car.

## What's currently in the game

### Driving system
- Controls work through standard keyboard input (`Vertical` / `Horizontal`) and also through on-screen buttons (gas, brake, left, right) so it plays properly on mobile, not just desktop.
- `motorTorque` is split between the two rear wheels (RR/RL), with a small formula that shifts torque between them based on the current steering angle and speed — so turning actually feels like a car shifting weight instead of a tank pivoting in place.
- Steering angle is driven by an `AnimationCurve` mapped against speed (`steeringCurve`), so the faster you go, the calmer and less twitchy the steering becomes, instead of turning just as sharply at 20 km/h and 150 km/h.
- There's a slip angle calculation (difference between the car's forward direction and its actual velocity direction) that gets used to nudge the steering correction while the car is sliding.

### Braking logic
- Braking isn't a flat on/off switch. There's logic using `Vector3.Dot` between the car's forward vector and its velocity to tell the difference between "pressing brake while moving forward" and "trying to reverse while still moving forward" (or the opposite case) — so the brakes actually respond correctly in both situations instead of the car just freezing awkwardly.
- An earlier version (still visible commented out in the code) only triggered braking based on slip angle, but that broke down specifically when reversing, which is why the current direction-based version replaced it.

### Fuel system
- Each car burns fuel gradually based on speed — drive faster, burn through the tank quicker — and the current fuel level persists between sessions via `PlayerPrefs`, so it doesn't reset every time the app closes.
- When the tank hits empty, the car is fully disabled (gas input zeroes out and the brakes kick in automatically) until the player refuels.
- There's a refill button that calculates price dynamically based on how much fuel is missing, and it auto-disables itself once the tank is above a certain threshold so players can't top off a tank that's basically already full.

### UI
- The speedometer reads real velocity straight from the `Rigidbody`, not a fake animated number, converted to an approximate km/h value.
- A fuel readout shows remaining liters directly, tied to the same tank logic driving the refuel button.

## Still in progress / experiments

- Top speed is calculated from each car's `motorPower`, so it scales per-vehicle instead of being one hardcoded number — still tuning the formula to make sure it stays believable across different engine strengths.
- There's a wheel particle system (smoke on slip) that's currently disabled in code — performance needs work before I bring it back.
- Actively tuning how turning feels at higher speeds; the current formula is a big improvement over earlier attempts but still not where I want it.

## Why this matters to me

I'm not using any pre-built vehicle physics asset — everything here is hand-tuned on top of Unity's base `WheelCollider`. That means every weird behavior I run into forces me to actually understand the physics behind it instead of copy-pasting a fix. This project is less "finished game" and more an ongoing physics playground, and the game itself is just how I test each idea in practice.

If you download it and something about the driving feels off, that's expected — it's still very much a work in progress and gets tweaked constantly.
