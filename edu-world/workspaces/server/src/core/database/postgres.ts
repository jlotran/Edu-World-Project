import postgresPlugin from 'typeorm-fastify-plugin';

import { appConfig } from '..';
import { Car } from '../../entities/Car';
import { Clothes } from '../../entities/Clothes';
import { JobLevel } from '../../entities/JobLevel';
import { PlayerCar } from '../../entities/PlayerCar';
import { PlayerClothes } from '../../entities/PlayerClothes';
import { Quest } from '../../entities/Quest';
import { User } from '../../entities/User';
import { AppRegister } from '../types';

export const postgresRegister: AppRegister = (app) => {
  app.register(postgresPlugin, {
    namespace: 'typeorm',
    type: 'postgres',
    host: appConfig.POSTGRES_HOST,
    port: appConfig.POSTGRES_PORT,
    username: appConfig.POSTGRES_USERNAME,
    password: appConfig.POSTGRES_PASSWORD,
    database: appConfig.POSTGRES_DATABASE,
    ssl: {
      rejectUnauthorized: true,
      ca: `-----BEGIN CERTIFICATE-----
MIIETTCCArWgAwIBAgIUK/bc0BNdfAXKbN5PVD7vP1zdlPQwDQYJKoZIhvcNAQEM
BQAwQDE+MDwGA1UEAww1NDlmMGZlMDMtMGM1OC00NjdjLWJiYmQtMWJlODA2NmVk
ZTYyIEdFTiAxIFByb2plY3QgQ0EwHhcNMjUwMzIwMDcxODIyWhcNMzUwMzE4MDcx
ODIyWjBAMT4wPAYDVQQDDDU0OWYwZmUwMy0wYzU4LTQ2N2MtYmJiZC0xYmU4MDY2
ZWRlNjIgR0VOIDEgUHJvamVjdCBDQTCCAaIwDQYJKoZIhvcNAQEBBQADggGPADCC
AYoCggGBAK8StPJKB/+o05yk01deM9PFN3o3SG0hpzxRAJL+Neqx/AhTfNAO/Y0J
nvpJCKalua1NI7LqpI7MWhihlVcPJLgUZK6NhNpMhUt3OvbYlyehddKEvokiu/Xv
oNjmF5f1zUFBltgTCvQhGYbAxJTKKl00hOGvY0Dn3e0l7IR133dkYlDZBCwv+PQ4
duM44Va3pkqldC5EDggDDLlEFYGBfADGqgK+Fi97PvQk4rtpOvNwPFqmQ7oM27+T
oxpNKy2LPcII8UmiEcbjZsuDKEbjv6FnzNtUBcIeQAjpa+iMAW2+Ti3KR44kL4ii
mR9rxUUDdFSUpWsYa71t7ueUvRMPormOTNOFqDIYdYlXajrSs7dTLIHIz6jWGd6N
XHmYiTQnEzhxqHBzWEN0R+y+ro2HC89YFac+HbMuHJOZmKodSR8FPkoDeIkQJgef
tm9OK9+qfmPSAK34btdFvOG2AyIvun5CwDHofDNYkx/Pr3/3ktlJ+BAfDQR4q6ZO
rLJO2eoWIQIDAQABoz8wPTAdBgNVHQ4EFgQUCkmFRF68Vq+iANb+PaUWlkK8ec4w
DwYDVR0TBAgwBgEB/wIBADALBgNVHQ8EBAMCAQYwDQYJKoZIhvcNAQEMBQADggGB
AEai7C7DuDi07cj3lPfMvph0jE3TDM8VYcTiRRtJ7ZUb4GEqTtni8l3jJQkhNbDk
T49OiOWSId9FLgsa8tzVrGYN21Lqb2039mzMjggRdMwmkbcyzKu3Nt3G/a447Td8
3aolbEUSVRQxfI2IiSALRw+0iJGx51YXZmC6XthnPRBPKuZl/nRrUFXAWnTIf4YG
R9eI8neo27h+gwMgQNAq//7Jb5BWz5NKp0QhpsORbtKO9Dhvkl6oCmefcuOd0JUq
c6ZdPikN/62+8Lbv9Pgi53zeSlHjSYN9k9V+u+5xHgmVlift5KhAHygh5PM0Xs/K
5KGskoGucK7hO+w0O4kixC+GGedQIFljfPjaYuU8k3LXTXh8i7k4+miKiktUs0+S
SCmKG7+DQyjFRNAZRfGFUaEMlagrYDia+CTZHJ++6z0ETS2ifi6AGfhKrKWk58jV
QsgJCvF9h2ZH8k/VsnYJQm9T91YqeYBaef24wOkf6xdZ2D1mbANCURQQ6IfI+ZEP
rw==
-----END CERTIFICATE-----`,
    },
    synchronize: true,
    logging: true,
    migrations: [__dirname + '/migration/*.ts'],
    entities: [User, JobLevel, Car, Clothes, Quest, PlayerCar, PlayerClothes],
  });
};
