import { Column, CreateDateColumn, Entity, ManyToOne, OneToMany, PrimaryGeneratedColumn, UpdateDateColumn } from 'typeorm';

@Entity({ name: 'quest' })
export class Quest {
  @PrimaryGeneratedColumn('uuid')
  id!: string;

  @Column({type: 'varchar', length: 255, nullable: false})
  name!: string;

  @Column({type: 'varchar', length: 255, nullable: false})
  description!: string;

  @Column({type: 'int', nullable: false})
  reward_exp!: number

  @Column({type: 'int', nullable: false})
  currency!: number

  @Column({type: 'varchar', nullable: true})
  item!: string

  @Column({ type: 'date', nullable: false })
  time_finish!: Date;
}
