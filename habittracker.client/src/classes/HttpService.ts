import { httpOptions, HttpServiceRes, Param, Header, IHttpService } from "./IHttpService";

export class HttpService implements IHttpService {

    public async Request<T>(url: string, options: httpOptions): Promise<HttpServiceRes<T>> {
        const {
            method,
            params,
            headers,
            body
        } = options;

        const requestUrl = this.BuildUrl(url, params || []);
        const requestHeaders = this.BuildHeaders(headers || []);
        
        const res = await fetch(
            requestUrl,
            {
                method,
                headers: requestHeaders,
                body
            }
        );

        const contentType = res.headers.get("Content-Type");
        const hasJson = contentType?.includes("application/json");
        const text = await res.text();
        
        let data = null;
        if (hasJson && text.length > 0) {
            data = JSON.parse(text);
        }

        const errorMessage = res.ok ? undefined : await res.statusText; 

        return {
            success: res.ok,
            status: res.status,
            data,
            errorMessage
        }
    }

    private BuildUrl(url: string, params: Param[]): string {
        const requestUrl = new URL(url);

        if (params.length > 0) {
            params.forEach(v => requestUrl.searchParams.append(v.key, v.value));
        }

        return requestUrl.toString();
    }

    private BuildHeaders(headers: Header[]): Headers {
        const reqHeaders = new Headers();

        for (const header of headers) {
            reqHeaders.append(header.key, header.value);
        }

        return reqHeaders;
    }
}