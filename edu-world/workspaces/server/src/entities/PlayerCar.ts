import { Column, Entity, ManyToOne, PrimaryGeneratedColumn, CreateDateColumn, JoinColumn } from 'typeorm';

import { Car } from './Car';
import { User } from './User';

@Entity({ name: 'player_cars' })
export class PlayerCar {
  @PrimaryGeneratedColumn('uuid')
  id!: string;

  @ManyToOne(() => User, (player) => player.id)
  @JoinColumn({ name: 'player_id' })
  player_id!: User;

  @ManyToOne(() => Car, (car) => car.car_id)
  @JoinColumn({ name: 'car_id' })
  car_id!: Car;

  @Column({ type: 'boolean' , default: false})
  equipped!: boolean;

  @Column({type: 'boolean', default: false})
  is_deleted!: number

  @CreateDateColumn()
  created_at!: Date;

  @CreateDateColumn()
  updated_at!: Date;
}
