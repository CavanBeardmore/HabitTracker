import {afterEach, beforeEach, describe, expect, it, vi} from "vitest";
import {AuthenticationService} from "../../src/classes/AuthenticationService";
import {expiredToken, testNonExpiryJwt, validExpiryToken} from "../utils/jwt";

describe("AuthenticationService", () => {
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

    describe("Login", () => {
        it("should return false with message when request fails with message", async () => {
            const headers = new Headers();
            headers.set("Content-Type", "application/json");
            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                ok: false,
                json:() => Promise.resolve({Message: "test-error"})
            });

            const service = new AuthenticationService("http://test-url");
            const [success, error] = await service.Login({username: "test", password: "test"});

            expect(success).toBe(false);
            expect(error).toBe("test-error");
        })

        it("should return false with set message when message doesnt exist", async () => {
            const headers = new Headers();
            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                ok: false,
            });

            const service = new AuthenticationService("http://test-url");
            const [success, error] = await service.Login({username: "test", password: "test"});

            expect(success).toBe(false);
            expect(error).toBe("An error has occurred.");
        })

        it("should return false when request is ok but data is null", async () => {
            const headers = new Headers();
            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                ok: true,
            });

            const service = new AuthenticationService("http://test-url");
            const [success, error] = await service.Login({username: "test", password: "test"});

            expect(success).toBe(false);
            expect(error).toBe("An error has occurred.");
        })

        it("should return false when request is ok but data has no token", async () => {
            const headers = new Headers();
            headers.set("Content-Type", "application/json");
            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                ok: true,
                json:() => Promise.resolve({Message: "test-error"})
            });

            const service = new AuthenticationService("http://test-url");
            const [success, error] = await service.Login({username: "test", password: "test"});

            expect(success).toBe(false);
            expect(error).toBe("test-error");
        })

        it("should return true when request is ok and data has token", async () => {
            const headers = new Headers();
            headers.set("Content-Type", "application/json");
            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                ok: true,
                json:() => Promise.resolve({token: "test-token"})
            });

            const service = new AuthenticationService("http://test-url");
            const [success, error] = await service.Login({username: "test", password: "test"});

            expect(success).toBe(true);
            expect(error).toBe("");
            expect(storageSetMock).toBeCalledWith("AUTH_TOKEN", "test-token");
        })
    })

    describe("IsUserAuthed", () => {
        it("should return false if local storage doesnt have auth token", () => {
            vi.mocked(storageGetMock).mockReturnValue(null);

            const service = new AuthenticationService("http://test-url");
            const result = service.IsUserAuthed();

            expect(result).toBe(false);
        })

        it("should return false if expiry is undefined", () => {
            vi.mocked(storageGetMock).mockReturnValue(testNonExpiryJwt);

            const service = new AuthenticationService("http://test-url");
            const result = service.IsUserAuthed();

            expect(result).toBe(false);
            expect(storageRemoveMock).toBeCalledWith("AUTH_TOKEN")
        })

        it("should return false if expired", () => {
            vi.mocked(storageGetMock).mockReturnValue(expiredToken);

            const service = new AuthenticationService("http://test-url");
            const result = service.IsUserAuthed();

            expect(result).toBe(false);
            expect(storageRemoveMock).toBeCalledWith("AUTH_TOKEN")
        })

        it("should return true if not expired", () => {
            vi.mocked(storageGetMock).mockReturnValue(validExpiryToken);

            const service = new AuthenticationService("http://test-url");
            const result = service.IsUserAuthed();

            expect(result).toBe(true);
        })
    })

    describe("RemoveAuthToken", () => {
        it("should call removeItem with AUTH_TOKEN", () => {
            const service = new AuthenticationService("http://test-url");
            service.RemoveAuthToken();

            expect(storageRemoveMock).toBeCalledWith("AUTH_TOKEN");
        })
    })
})