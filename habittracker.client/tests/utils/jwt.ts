import jwt from 'jsonwebtoken';

const payload = {
  name: "cbeardmore",
};

const secret = "your-secret-key";

export const validExpiryToken = jwt.sign(payload, secret, {
  expiresIn: '1h'
});

const expiredPayload = {
  name: "cbeardmore",
  exp: Math.floor(Date.now() / 1000) - 3600
};

export const expiredToken = jwt.sign(expiredPayload, secret, {});

export const testNonExpiryJwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.";