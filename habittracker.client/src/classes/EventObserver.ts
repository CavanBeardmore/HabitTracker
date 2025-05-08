export abstract class EventObserver {
    protected readonly _events: Map<string, Map<string, Function>> = new Map();

    public add(name: string, id: string, callback: Function): void {
        if (!this._events.has(name)) {
            this._events.set(name, new Map<string, Function>());
        }

        this._events.get(name)!.set(id, callback);
    }

    public remove(name: string, id: string): void {
        const events = this._events.get(name);

        if (!events) return;

        events.delete(id);

        if (events.size === 0) {
            this._events.delete(name);
        }
    }

    public raise(name: string, ...args: unknown[]): void {
        const events = this._events.get(name);

        if (events) {
            const callbacks = events.values();
            for (const callback of callbacks) {
                callback(...args);
            }
        }
    }
}