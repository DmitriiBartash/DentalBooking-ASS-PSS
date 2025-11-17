import encoding from 'k6/encoding';
import http from 'k6/http';
import { check, group } from 'k6';

export const options = {
  vus: 200,
  duration: '2m',
  thresholds: {
    http_req_duration: ['p(95)<2000'],
  },
};

const BASE_URL = 'http://gateway:8080/api';
const USERS = [
  { email: 'client1@dental.local', password: 'Client123!', role: 'Client' },
  { email: 'client2@dental.local', password: 'Client123!', role: 'Client' }
];

function parseJwt(token) {
  try {
    const parts = token.split('.');
    if (parts.length < 2) return {};
    const decoded = encoding.b64decode(
      parts[1].replace(/-/g, '+').replace(/_/g, '/'),
      'std',
      's'
    );
    return JSON.parse(decoded);
  } catch {
    return {};
  }
}

export default function () {
  const user = USERS[Math.floor(Math.random() * USERS.length)];

  const headers = {
    'Content-Type': 'application/json',
    'X-Request-Source': 'WebClient',
  };

  const loginRes = http.post(
    `${BASE_URL}/auth/login`,
    JSON.stringify({ email: user.email, password: user.password }),
    { headers }
  );

  const token = loginRes.json('token');

  check(loginRes, {
    'login 200': (r) => r.status === 200,
  });

  if (!token) {
    return;
  }

  const authHeaders = {
    ...headers,
    Authorization: `Bearer ${token}`,
  };

  const doctorsRes = http.get(`${BASE_URL}/doctor`, { headers: authHeaders });
  check(doctorsRes, { 'doctor ok': (r) => r.status === 200 });

  const procRes = http.get(`${BASE_URL}/procedure`, { headers: authHeaders });
  check(procRes, { 'procedure ok': (r) => r.status === 200 });

  const payload = parseJwt(token);
  const clientId =
    payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

  if (!clientId) {
    return;
  }

  const body = JSON.stringify({
    doctorId: 1,
    procedureId: 1,
    clientId: clientId,
    startUtc: new Date(Date.now() + 3600 * 1000).toISOString(),
    notes: `PerfTest ${__VU}-${__ITER}`,
  });

  const bookingRes = http.post(`${BASE_URL}/booking/create`, body, {
    headers: authHeaders,
  });

  check(bookingRes, {
    'booking ok': (r) => [200, 201].includes(r.status),
  });
}
