import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { HabitService } from "../../src/classes/HabitService";
import { Habit } from "../../src/data/Habit";

describe("HabitService", () => {
    const fetchMock = vi.fn();
    const storageSetMock = vi.fn();
    const storageGetMock = vi.fn();
    const storageRemoveMock = vi.fn();

    beforeEach(() => {
        global.fetch = fetchMock;
        global.localStorage ={
            setItem: storageSetMock,
            getItem: storageGetMock,
            removeItem: storageRemoveMock
        } as Partial<Storage> as Storage
    });

    afterEach(() => {
        vi.restoreAllMocks();
    });

    describe("CreateHabit", () => {
        it("should call fetch with correct arguments", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            vi.mocked(fetchMock).mockResolvedValue({
                ok: true,
                status: 200,
                headers: {
                get: (key: string) =>
                    key.toLowerCase() === "content-type" ? "application/json" : null,
                },
                json: async () => ({}),
            });

            const service = new HabitService("http://test-url/");

            await service.CreateHabit("test");

            const expectedHeaders = new Headers();
            expectedHeaders.append("Authorization", "Bearer test");
            expectedHeaders.append("Content-Type", "application/json");

            expect(fetchMock).toHaveBeenCalledTimes(1);

            const [url, options] = fetchMock.mock.calls[0];

            expect(url).toBeInstanceOf(URL);
            expect((url as URL).href).toBe("http://test-url/habits/Habit");
            expect(options.method).toBe("POST");
            expect(options.body).toBe('{"name":"test"}');

            expect(options.headers).toBeInstanceOf(Headers);
            expect(options.headers.get("Authorization")).toBe("Bearer test");
            expect(options.headers.get("Content-Type")).toBe("application/json");
        })
    })

    describe("GetHabits", () => {
        it("should return array of habits when successful", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            const headers = new Headers();
            headers.set("Content-Type", "application/json");

            const jsonData: Habit[] = [
                {
                    Id: 1,
                    User_id: 2,
                    StreakCount: 3,
                    Name: "test"
                }
            ];
            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                status: 200,
                ok: true,
                json:() => Promise.resolve(jsonData)
            });

            const service = new HabitService("http://test-url/");

            const {success, status, data, errorMessage} = await service.GetHabits();

            expect(success).toBe(true);
            expect(status).toBe(200);
            expect(errorMessage).toBe(undefined);
            expect(data).toStrictEqual(jsonData);
        })

        it("should return null when failed", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            const headers = new Headers();

            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                status: 500,
                ok: false,
                statusText: "test"
            });

            const service = new HabitService("http://test-url/");

            const {success, status, data, errorMessage} = await service.GetHabits();

            expect(success).toBe(false);
            expect(status).toBe(500);
            expect(errorMessage).toBe("test");
            expect(data).toBe(null);
        })
    })

    describe("GetHabitById", () => {
        it("should return array of habits when successful", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            const headers = new Headers();
            headers.set("Content-Type", "application/json");

            const jsonData: Habit = 
            {
                Id: 1,
                User_id: 2,
                StreakCount: 3,
                Name: "test"
            };

            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                status: 200,
                ok: true,
                json:() => Promise.resolve(jsonData)
            });

            const service = new HabitService("http://test-url/");

            const {success, status, data, errorMessage} = await service.GetHabitById(1);

            expect(success).toBe(true);
            expect(status).toBe(200);
            expect(errorMessage).toBe(undefined);
            expect(data).toStrictEqual(jsonData);
        })

        it("should return null when failed", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            const headers = new Headers();

            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                status: 500,
                ok: false,
                statusText: "test"
            });

            const service = new HabitService("http://test-url/");

            const {success, status, data, errorMessage} = await service.GetHabitById(1);

            expect(success).toBe(false);
            expect(status).toBe(500);
            expect(errorMessage).toBe("test");
            expect(data).toBe(null);
        })
    })

    describe("UpdateHabit", () => {
        it("should call fetch with correct args", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            const headers = new Headers();
            headers.set("Content-Type", "application/json");

            const jsonData: Habit = 
            {
                Id: 1,
                User_id: 2,
                StreakCount: 3,
                Name: "test"
            };
            
            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                status: 200,
                ok: true,
                json:() => Promise.resolve(jsonData)
            });

            const service = new HabitService("http://test-url/");
            await service.UpdateHabit(1, "test");

            const expectedHeaders = new Headers();
            expectedHeaders.append("Authorization", "Bearer test");
            expectedHeaders.append("Content-Type", "application/json");

            expect(fetchMock).toHaveBeenCalledTimes(1);

            const [url, options] = fetchMock.mock.calls[0];

            expect(url).toBeInstanceOf(URL);
            expect((url as URL).href).toBe("http://test-url/habits/Habit");
            expect(options.method).toBe("PATCH");
            expect(options.body).toBe('{"id":1,"name":"test"}');

            expect(options.headers).toBeInstanceOf(Headers);
            expect(options.headers.get("Authorization")).toBe("Bearer test");
            expect(options.headers.get("Content-Type")).toBe("application/json");
        })
    })

    describe("DeleteHabit", () => {
        it("should call fetch with correct args", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            const headers = new Headers();
            headers.set("Content-Type", "application/json");

            const jsonData: Habit = 
            {
                Id: 1,
                User_id: 2,
                StreakCount: 3,
                Name: "test"
            };
            
            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                status: 200,
                ok: true,
                json:() => Promise.resolve(jsonData)
            });

            const service = new HabitService("http://test-url/");
            await service.DeleteHabit(1);

            const expectedHeaders = new Headers();
            expectedHeaders.append("Authorization", "Bearer test");
            expectedHeaders.append("Content-Type", "application/json");

            expect(fetchMock).toHaveBeenCalledTimes(1);

            const [url, options] = fetchMock.mock.calls[0];

            expect(url).toBeInstanceOf(URL);
            expect((url as URL).href).toBe("http://test-url/habits/Habit?habitId=1");
            expect(options.method).toBe("DELETE");

            expect(options.headers).toBeInstanceOf(Headers);
            expect(options.headers.get("Authorization")).toBe("Bearer test");
        })
    })
})