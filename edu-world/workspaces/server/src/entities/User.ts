import { Column, CreateDateColumn, Entity, PrimaryGeneratedColumn, UpdateDateColumn, ManyToOne } from 'typeorm';

import { JobLevel } from './JobLevel';

@Entity({ name: 'users' })
export class User {
  @PrimaryGeneratedColumn('uuid')
  id!: string;

  @Column({ type: 'varchar', length: 255, unique: true })
  username!: string;

  @Column({ type: 'int', default: 0 })
  currency!: number;

  @ManyToOne(() => JobLevel, (jobLevel) => jobLevel.id)
  level!: JobLevel; 
  
  @Column({ type: 'int', default: 0 })
  experience!: number;

  @Column({ type: 'int', default: 0 })
  matches_played!: number;

  @Column({ type: 'varchar', length: 255, nullable: true })
  avatar!: string;

  @Column({ type: 'decimal', precision: 5, scale: 2, default: 0 })
  win_rate!: number;  // Định nghĩa kiểu decimal(5, 2)
  
  @CreateDateColumn()
  created_at!: Date;

  @UpdateDateColumn()
  updated_at!: Date;
}
