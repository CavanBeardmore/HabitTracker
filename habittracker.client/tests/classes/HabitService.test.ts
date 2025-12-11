import { describe, expect, it, vi } from "vitest";
import { HttpServiceRes, IHttpService, RequestMethod } from "../../src/classes/IHttpService";
import { HabitService } from "../../src/classes/HabitService";
import { before, beforeEach } from "node:test";

const Request = vi.fn();

const mockHttpService = {
    Request
} as IHttpService;

const habitService = new HabitService(
    "/testurl",
    mockHttpService,
);

describe("HabitService", () => {
    beforeEach(() => {
        vi.clearAllMocks();
    })

    describe("CreateHabit", () => {
        it("should call Request from http service with correct args", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            await habitService.CreateHabit("test-habit");

            expect(Request).toBeCalledWith(
                "/testurl/Habit",
                {
                    method: RequestMethod.POST,
                    headers: [
                        {
                            key: "Content-Type", 
                            value: "application/json"
                        }
                    ],
                    body: JSON.stringify({name: "test-habit"})
                }
            );
        })
    })
    
    describe("GetHabits", () => {
        it("should call Request from http service with correct args", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            await habitService.GetHabits();

            expect(Request).toBeCalledWith(
                "/testurl/Habit",
                {
                    method: RequestMethod.GET,
                    headers: [
                        {
                            key: "Content-Type", 
                            value: "application/json"
                        }
                    ],
                }
            );
        })

        it("should return result from http service Request", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            const result = await habitService.GetHabits();

            expect(result).toStrictEqual(data);
        })
    })

    describe("GetHabitById", () => {
        it("should call Request from http service with correct args", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            await habitService.GetHabitById(1);

            expect(Request).toBeCalledWith(
                "/testurl/Habit/1",
                {
                    method: RequestMethod.GET,
                    headers: [
                        {
                            key: "Content-Type", 
                            value: "application/json"
                        }
                    ],
                }
            );
        })

        it("should return result from http service Request", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            const result = await habitService.GetHabitById(1);

            expect(result).toStrictEqual(data);
        })
    })

    describe("UpdateHabit", () => {
        it("should call Request from http service with correct args", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            const id = 1;
            const name = "test-name";
            const streakCount = 3;

            await habitService.UpdateHabit(id, name, streakCount);

            expect(Request).toBeCalledWith(
                "/testurl/Habit",
                {
                    method: RequestMethod.PATCH,
                    headers: [
                        {
                            key: "Content-Type", 
                            value: "application/json"
                        }
                    ],
                        body: JSON.stringify({
                        id,
                        name,
                        streakCount
                    })
                }
            );
        });
    })

    describe("DeleteHabit", () => {
        it("should call Request from http service with correct args", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            const id = 1;

            await habitService.DeleteHabit(id);

            expect(Request).toBeCalledWith(
                "/testurl/Habit/1",
                {
                    method: RequestMethod.DELETE,
                }
            );
        });
    })
})