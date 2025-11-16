import http from 'k6/http';
import { sleep, check } from 'k6';

export let options = {
    vus: 200,
    duration: '2m',
    insecureSkipTLSVerify: true,
};

const BASE = "https://localhost:44367";

function extractToken(html) {
    let match = html.match(/name="__RequestVerificationToken" type="hidden" value="(.+?)"/);
    return match ? match[1] : "";
}

export default function () {
    let page = http.get(`${BASE}/Login`);
    let cookies = page.cookies;
    let token = extractToken(page.body);

    let loginRes = http.post(
        `${BASE}/Login`,
        `Email=client1@dental.com&Password=Client123!&__RequestVerificationToken=${token}`,
        {
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            cookies
        }
    );

    check(loginRes, { "login succeeded": (r) => r.status === 200 || r.status === 302 });

    cookies = loginRes.cookies;

    let formRes = http.get(`${BASE}/Client/CreateBooking`, { cookies });
    check(formRes, { "booking page loaded": (r) => r.status === 200 || r.status === 302 });

    let formToken = extractToken(formRes.body);

    let start = new Date(Date.now() + 600000);
    let date = start.toISOString().split("T")[0];
    let time = start.toTimeString().split(" ")[0];

    let bookingRes = http.post(
        `${BASE}/Client/CreateBooking`,
        `Input.ProcedureId=1&Input.DoctorId=1&Input.Date=${date}&Input.Time=${time}&__RequestVerificationToken=${formToken}`,
        {
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            cookies
        }
    );

    check(bookingRes, {
        "booking created or redirected": (r) =>
            r.status === 200 || r.status === 302
    });

    sleep(1);
}
