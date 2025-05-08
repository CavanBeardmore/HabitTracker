import { Resolve } from "@here-mobility/micro-di"
import { HabitService } from "../classes/HabitService"

export const Home = () => {
    const habitService = Resolve<HabitService>(HabitService);
    return (
        <div>
            <p>Home</p>
            <button className="bg-black text-white" onClick={() => habitService.CreateHabit("test123")}>Press me</button>
        </div>
    )
}