import encoding from 'k6/encoding';
import http from 'k6/http';
import { sleep, check, group } from 'k6';

export const options = {
  stages: [
    { duration: '10s', target: 10 },
    { duration: '20s', target: 20 },
    { duration: '10s', target: 0 },
  ],
  thresholds: {
    http_req_duration: ['p(95)<1000'],
  },
};

const BASE_URL = 'http://gateway:8080/api';
const USERS = [
  { email: 'admin@dental.local', password: 'Admin123!', role: 'Admin' },
  { email: 'client1@dental.local', password: 'Client123!', role: 'Client' },
  { email: 'client2@dental.local', password: 'Client123!', role: 'Client' }
];

function parseJwt(token) {
  try {
    const parts = token.split('.');
    if (parts.length < 2) return {};
    const payloadBase64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    const decoded = encoding.b64decode(payloadBase64, 'std', 's');
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

  const loginRes = http.post(`${BASE_URL}/auth/login`, JSON.stringify({
    email: user.email,
    password: user.password,
  }), { headers });

  const token = loginRes.json('token');

  check(loginRes, {
    'login 200': (r) => r.status === 200,
    'token valid': () => !!token,
  });

  if (!token) {
    sleep(1);
    return;
  }

  const authHeaders = {
    ...headers,
    Authorization: `Bearer ${token}`,
  };

  group('Get doctors', () => {
    const res = http.get(`${BASE_URL}/doctor`, { headers: authHeaders });
    let data = [];
    try {
      data = res.json();
    } catch {}
    check(res, {
      'doctor 200': (r) => r.status === 200,
      'doctor not empty': () => Array.isArray(data) && data.length > 0,
    });
  });

  group('Get procedures', () => {
    const res = http.get(`${BASE_URL}/procedure`, { headers: authHeaders });
    check(res, { 'procedure 200': (r) => r.status === 200 });
  });

  if (user.role === 'Client') {
    group('Create booking', () => {
      const payload = parseJwt(token);
      const clientId =
        payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
        payload.nameid ||
        payload.userId ||
        null;

      if (!clientId) return;

      const body = JSON.stringify({
        doctorId: 1,
        procedureId: 1,
        clientId: clientId,
        startUtc: new Date(Date.now() + 3600 * 1000).toISOString(),
        notes: `Load test booking ${__VU}-${__ITER}`,
      });

      const res = http.post(`${BASE_URL}/booking/create`, body, { headers: authHeaders });

      check(res, {
        'booking status ok': (r) => [200, 201].includes(r.status),
      });
    });
  }

  sleep(1);
}
