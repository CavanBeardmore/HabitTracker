import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { UserService } from "../../src/classes/UserService";

describe("UserService", () => {
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

    describe("CreateUser", () => {
        it("should call fetch with correct arguments", async () => {
            vi.mocked(fetchMock).mockResolvedValue({
                ok: true,
                status: 200,
                headers: {
                get: (key: string) =>
                    key.toLowerCase() === "content-type" ? "application/json" : null,
                },
                json: async () => ({}),
            });

            const service = new UserService("http://test-url/");

            await service.CreateUser({
                email: "test",
                username: "test",
                password: "test"
            });

            const expectedHeaders = new Headers();
            expectedHeaders.append("Authorization", "Bearer test");
            expectedHeaders.append("Content-Type", "application/json");

            expect(fetchMock).toHaveBeenCalledTimes(1);

            const [url, options] = fetchMock.mock.calls[0];

            expect(url).toBeInstanceOf(URL);
            expect((url as URL).href).toBe("http://test-url/user/User");
            expect(options.method).toBe("POST");
            expect(options.body).toBe('{"email":"test","username":"test","password":"test"}');

            expect(options.headers).toBeInstanceOf(Headers);
            expect(options.headers.get("Content-Type")).toBe("application/json");
        })
    })
})